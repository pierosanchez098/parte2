package com.example.feature_perfil

import android.Manifest
import android.content.ContentValues
import android.content.Context
import android.content.Intent
import android.graphics.Bitmap
import android.net.Uri
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Book
import androidx.compose.material.icons.filled.Class
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import coil.compose.AsyncImage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import android.provider.MediaStore
import android.util.Log
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.offset
import androidx.compose.material.icons.filled.PhotoCamera
import androidx.compose.material3.IconButton
import androidx.compose.material3.IconButtonDefaults
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.graphics.Color
import com.google.accompanist.permissions.rememberPermissionState
import java.io.ByteArrayOutputStream
import androidx.compose.ui.platform.LocalContext
import com.google.accompanist.permissions.ExperimentalPermissionsApi
import com.google.accompanist.permissions.isGranted
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch
import java.io.File
import java.io.FileOutputStream
import android.util.Base64
import androidx.activity.result.launch
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import com.example.core.R
import com.example.core.utils.Device
import com.example.core.utils.DeviceTracker
import com.example.core.utils.ImageUtils
import com.example.data.GestorSQLExternModern
import com.example.data.UnsafeSSL
import com.example.domain.model.PerfilData
import org.json.JSONArray
import com.example.data.SecureSessionManager

@OptIn(ExperimentalPermissionsApi::class)
@Composable
fun PerfilScreen(dniPersona: String, modifier: Modifier = Modifier) {
    val context = LocalContext.current
    var perfil by remember { mutableStateOf<PerfilData?>(null) }
    var isLoading by remember { mutableStateOf(true) }
    var error by remember { mutableStateOf<String?>(null) }

    val cameraPermissionState = rememberPermissionState(Manifest.permission.CAMERA)

    val coroutineScope = rememberCoroutineScope()

    val cameraLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.TakePicturePreview()
    ) { bitmap ->
        if (bitmap != null) {
            val imagenProcesada = ImageUtils.procesarImagen(bitmap)
            val bytes = ImageUtils.comprimirImagen(imagenProcesada)

            ImageUtils.guardarEnGaleria(context, imagenProcesada)

            coroutineScope.launch {
                val uriLocal = withContext(Dispatchers.IO) {
                    ImageUtils.obtenerUriDeImagen(context, imagenProcesada)
                }
                perfil = perfil?.copy(foto = uriLocal.toString())

                val exitoSubida = withContext(Dispatchers.IO) {
                    ImageUtils.subirFotoAlServidor(dniPersona, bytes)
                }

                if (exitoSubida) {
                    Toast.makeText(
                        context,
                        context.getString(R.string.profile_toast_photo_updated),
                        Toast.LENGTH_SHORT
                    ).show()
                } else {
                    Toast.makeText(
                        context,
                        context.getString(R.string.profile_toast_photo_error),
                        Toast.LENGTH_LONG
                    ).show()
                }
            }
        }
    }


    LaunchedEffect(dniPersona) {
        if (dniPersona.isEmpty()) {
            error = "No se ha recibido el identificador del usuario"
            isLoading = false
            return@LaunchedEffect
        }

        isLoading = true
        error = null

        try {
            UnsafeSSL.ignoreSSLErrors()
            val gestor = GestorSQLExternModern()

            val sessionManager = SecureSessionManager(context)
            val token = sessionManager.getToken() ?: ""

            val tracker = DeviceTracker(context)
            val userAgentHash = tracker.getUserAgentHash()

            val response = withContext(Dispatchers.IO) {
                gestor.connectarObjPOST(
                    "http://10.0.2.2/get_perfil.php",
                    "token=$token&dni_persona=$dniPersona&user_agent_hash=$userAgentHash"
                )
            }

            if (response == null) {
                error = gestor.lastError ?: context.getString(R.string.profile_err_no_server_response)
            } else {
                if (response.optBoolean("expired", false)) {
                    val mensajeServidor = response.optString("error", null)

                    withContext(Dispatchers.Main) {
                        val deviceHandler = Device(context, sessionManager)
                        deviceHandler.forzarReLogin(mensajeServidor)
                    }
                    return@LaunchedEffect
                }

                val newToken = response.optString("new_token", "")
                if (newToken.isNotEmpty()) {
                    sessionManager.saveSession(newToken, dniPersona)
                }

                if (response.has("error") && !response.isNull("error")) {
                    error = response.getString("error")
                } else {
                    val datosJson = response.optJSONObject("datos")

                    if (datosJson != null) {
                        val asigJson = response.optJSONArray("asignaturas") ?: JSONArray()
                        val listaAsignaturas = mutableListOf<String>()

                        for (i in 0 until asigJson.length()) {
                            listaAsignaturas.add(asigJson.getString(i))
                        }

                        perfil = PerfilData(
                            nombre = datosJson.optString("nom", "Desconocido"),
                            apellidos = datosJson.optString("cognom", "Desconocido"),
                            nia = datosJson.optInt("nia", 0),
                            grupo = datosJson.optString("grupo", "Sin grupo").takeIf { it.isNotBlank() && it != "null" } ?: "Sin grupo",
                            aula = datosJson.optString("aula", "N/A").takeIf { it.isNotBlank() && it != "null" } ?: "N/A",
                            foto = datosJson.optString("foto", "N/A").takeIf { it.isNotBlank() && it != "null" } ?: "Sin foto",
                            asignaturas = listaAsignaturas
                        )
                    } else {
                        error = context.getString(R.string.profile_err_no_data_found)
                    }
                }
            }
        } catch (e: Exception) {
            error = "Excepción: ${e.message}"
        } finally {
            isLoading = false
        }
    }

    Box(modifier = modifier.fillMaxSize()) {
        if (isLoading) {
            CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
        } else if (error != null) {
            Text(text = "Error: $error", modifier = Modifier.align(Alignment.Center), color = MaterialTheme.colorScheme.error)
        } else {
            perfil?.let { data ->
                LazyColumn(
                    modifier = Modifier.fillMaxSize(),
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    item {
                        Card(
                            modifier = Modifier.fillMaxWidth(),
                            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.primaryContainer)
                        ) {
                            Row(modifier = Modifier.padding(16.dp), verticalAlignment = Alignment.CenterVertically) {
                                Box(contentAlignment = Alignment.BottomEnd) {
                                    if (data.foto != "Sin foto") {
                                        AsyncImage(
                                            model = data.foto,
                                            contentDescription = stringResource(id = R.string.profile_desc_photo),
                                            modifier = Modifier
                                                .size(80.dp)
                                                .clip(CircleShape)
                                                .clickable {
                                                    if (cameraPermissionState.status.isGranted) cameraLauncher.launch()
                                                    else cameraPermissionState.launchPermissionRequest()
                                                },
                                            contentScale = ContentScale.Crop
                                        )
                                    } else {
                                        Icon(
                                            Icons.Default.Person,
                                            contentDescription = null,
                                            modifier = Modifier
                                                .size(80.dp)
                                                .clip(CircleShape)
                                                .clickable {
                                                    if (cameraPermissionState.status.isGranted) cameraLauncher.launch()
                                                    else cameraPermissionState.launchPermissionRequest()
                                                }
                                        )
                                    }

                                    IconButton(
                                        onClick = {
                                            if (cameraPermissionState.status.isGranted) cameraLauncher.launch()
                                            else cameraPermissionState.launchPermissionRequest()
                                        },
                                        modifier = Modifier
                                            .size(28.dp)
                                            .offset(x = (4).dp, y = (4).dp)
                                            .shadow(2.dp, CircleShape),
                                        colors = IconButtonDefaults.iconButtonColors(
                                            containerColor = MaterialTheme.colorScheme.primary,
                                            contentColor = Color.White
                                        )
                                    ) {
                                        Icon(
                                            painter = painterResource(id = R.drawable.camara_ic),
                                            contentDescription = stringResource(id = com.example.core.R.string.profile_desc_camera),
                                            modifier = Modifier.size(16.dp)
                                        )
                                    }
                                }

                                Spacer(modifier = Modifier.width(16.dp))
                                Column {
                                    Text(text = "${data.nombre} ${data.apellidos}", style = MaterialTheme.typography.headlineSmall, fontWeight = FontWeight.Bold)
                                    Text(text = "NIA: ${data.nia}", style = MaterialTheme.typography.bodyMedium)
                                }
                            }
                        }
                    }

                    item {
                        Card(modifier = Modifier.fillMaxWidth()) {
                            Row(modifier = Modifier.padding(16.dp), verticalAlignment = Alignment.CenterVertically) {
                                Icon(
                                    painter = painterResource(id = com.example.feature_perfil.R.drawable.book),
                                    contentDescription = stringResource(id = com.example.core.R.string.profile_desc_book),
                                    modifier = Modifier.size(24.dp)
                                )
                                Spacer(modifier = Modifier.width(16.dp))
                                Column {
                                    Text(
                                        text = stringResource(R.string.profile_group, data.grupo),
                                        style = MaterialTheme.typography.titleMedium,
                                        fontWeight = FontWeight.Bold
                                    )
                                    Text(
                                        text = stringResource(R.string.profile_room, data.aula),
                                        style = MaterialTheme.typography.bodyMedium
                                    )
                                }
                            }
                        }
                    }


                }
            }
        }
    }
}
