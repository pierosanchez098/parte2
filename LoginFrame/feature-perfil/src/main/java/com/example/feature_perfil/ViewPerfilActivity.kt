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
import androidx.compose.material3.OutlinedCard
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
import androidx.compose.foundation.lazy.items
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.core.utils.GestorTema
import com.example.data.GestorSQLExternModern
import com.example.data.UnsafeSSL
import com.example.domain.model.PerfilData
import org.json.JSONArray
import kotlin.jvm.java
import com.example.data.SecureSessionManager


class ViewPerfilActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        val gestorTema = GestorTema(this)

        val sessionManager = SecureSessionManager(this)
        val dniPersona = sessionManager.getDni() ?: ""

        if (dniPersona.isEmpty()) {
            Toast.makeText(this, "Sesión no válida, vuelve a iniciar sesión.", Toast.LENGTH_LONG).show()
            val intent = Intent().setClassName(packageName, "com.example.loginframe.MainActivity")
            intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            startActivity(intent)
            finish()
            return
        }

        setContent {

            var isDark by remember { mutableStateOf(gestorTema.isDarkMode()) }

            LoginFrameTheme(darkTheme = isDark) {
                AppDrawerScaffold(
                    currentScreenTitle = "Mi perfil",
                    dniPersona = dniPersona,
                    isDarkMode = isDark,
                    onThemeChanged = { nuevoValor ->
                        isDark = nuevoValor
                        gestorTema.setDarkMode(nuevoValor)
                    }
                ) { padding ->
                    PerfilScreen(dniPersona, Modifier.padding(padding))

                }
            }
        }
    }
}

@OptIn(ExperimentalPermissionsApi::class)
@Composable
fun PerfilScreen(dniPersona: String, modifier: Modifier = Modifier) {
    val context = LocalContext.current
    var perfil by remember { mutableStateOf<PerfilData?>(null) }
    var isLoading by remember { mutableStateOf(true) }
    var error by remember { mutableStateOf<String?>(null) }

    val cameraPermissionState = rememberPermissionState(Manifest.permission.CAMERA)

    val cameraLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.TakePicturePreview()
    ) { bitmap ->
        if (bitmap != null) {
            val imagenProcesada = procesarImagen(bitmap)

            val bytes = comprimirImagen(imagenProcesada)

            guardarEnGaleria(context, imagenProcesada)

            val uriLocal = obtenerUriDeImagen(context, imagenProcesada)
            perfil = perfil?.copy(foto = uriLocal.toString())

            subirFotoAlServidor(dniPersona, bytes)

            Toast.makeText(context, "Foto actualizada correctamente", Toast.LENGTH_SHORT).show()
        }
    }


    LaunchedEffect(dniPersona) {
        withContext(Dispatchers.IO) {
            try {
                UnsafeSSL.ignoreSSLErrors()
                val gestor = GestorSQLExternModern()

                val sessionManager = SecureSessionManager(context)
                val token = sessionManager.getToken() ?: ""

                val response = gestor.connectarObjPOST(
                    "http://10.0.2.2/get_perfil.php",
                    "token=$token&dni_persona=$dniPersona"
                )

                if (response == null) {
                    error = gestor.lastError ?: "No se recibió respuesta del servidor"
                } else {
                    val newToken = response.optString("new_token", "")
                    if (newToken.isNotEmpty()) {
                        sessionManager.saveSession(newToken, dniPersona)
                    }

                    if (response.optBoolean("expired", false)) {
                        sessionManager.logout()

                        val intent = Intent().apply {
                            setClassName(context.packageName, "com.example.loginframe.MainActivity")
                            flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                        }
                        context.startActivity(intent)
                        return@withContext
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
                            error = "No se encontraron datos del perfil"
                        }
                    }
                }
            } catch (e: Exception) {
                error = "Excepción: ${e.message}"
            } finally {
                isLoading = false
            }
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
                                            contentDescription = "Foto de perfil",
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
                                        Icon(Icons.Default.PhotoCamera, contentDescription = null, modifier = Modifier.size(16.dp))
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
                                Icon(Icons.Default.Class, contentDescription = null, tint = MaterialTheme.colorScheme.primary)
                                Spacer(modifier = Modifier.width(16.dp))
                                Column {
                                    Text(text = "Grupo: ${data.grupo}", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
                                    Text(text = "Aula asignada: ${data.aula}", style = MaterialTheme.typography.bodyMedium)
                                }
                            }
                        }
                    }


                }
            }
        }
    }
}

fun procesarImagen(bitmap: Bitmap): Bitmap {
    val tamanyo = minOf(bitmap.width, bitmap.height)
    val x = (bitmap.width - tamanyo) / 2
    val y = (bitmap.height - tamanyo) / 2

    val cuadrado = Bitmap.createBitmap(bitmap, x, y, tamanyo, tamanyo)

    return Bitmap.createScaledBitmap(cuadrado, 512, 512, true)
}

fun comprimirImagen(bitmap: Bitmap): ByteArray {
    val flujoSalida = ByteArrayOutputStream()
    bitmap.compress(Bitmap.CompressFormat.JPEG, 75, flujoSalida)
    return flujoSalida.toByteArray()
}

fun guardarEnGaleria(contexto: Context, bitmap: Bitmap) {
    val valores = ContentValues().apply {
        put(MediaStore.Images.Media.DISPLAY_NAME, "perfil_${System.currentTimeMillis()}.jpg")
        put(MediaStore.Images.Media.MIME_TYPE, "image/jpeg")
        put(MediaStore.Images.Media.RELATIVE_PATH, "Pictures/LoginFrame")
    }

    val uri = contexto.contentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, valores)

    uri?.let { direccion ->
        contexto.contentResolver.openOutputStream(direccion).use { salida ->
            bitmap.compress(Bitmap.CompressFormat.JPEG, 90, salida!!)
        }
    }
}

fun obtenerUriDeImagen(context: Context, bitmap: Bitmap): Uri {
    val file = File(context.cacheDir, "temp_perfil.jpg")
    FileOutputStream(file).use { bitmap.compress(Bitmap.CompressFormat.JPEG, 100, it) }
    return Uri.fromFile(file)
}

fun subirFotoAlServidor(dni: String, bytes: ByteArray) {
    CoroutineScope(Dispatchers.IO).launch {
        try {
            val base64 = Base64.encodeToString(bytes, Base64.DEFAULT)
            val gestor = GestorSQLExternModern()
            gestor.enviarPost("http://10.0.2.2/subir_foto.php", mapOf("dni" to dni, "imagen" to base64))
        } catch (e: Exception) { Log.e("API", "Error subiendo: ${e.message}") }
    }
}