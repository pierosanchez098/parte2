package com.example.loginframe

import android.content.Intent
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
import androidx.compose.foundation.lazy.items
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
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SegmentedButtonDefaults.Icon
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
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import coil.compose.AsyncImage
import com.example.loginframe.components.AppDrawerScaffold
import com.example.loginframe.components.ui.theme.LoginFrameTheme
import com.example.loginframe.data.model.PerfilData
import com.example.loginframe.utils.GestorSQLExternModern
import com.example.loginframe.utils.GestorTema
import com.example.loginframe.utils.UnsafeSSL
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext

class ViewPerfilActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        val gestorTema = GestorTema(this)

        var dniPersona = intent.getStringExtra("DNI_PERSONA") ?: ""

        if (dniPersona.isEmpty()) {
            val prefs = getSharedPreferences("user_prefs", MODE_PRIVATE)
            dniPersona = prefs.getString("dni_persona", "") ?: ""
        }

        if (dniPersona.isEmpty()) {
            Toast.makeText(this, "Sesión no válida, vuelve a iniciar sesión.", Toast.LENGTH_LONG).show()
            startActivity(Intent(this, MainActivity::class.java))
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

@Composable
fun PerfilScreen(dniPersona: String, modifier: Modifier = Modifier) {
    var perfil by remember { mutableStateOf<PerfilData?>(null) }
    var isLoading by remember { mutableStateOf(true) }
    var error by remember { mutableStateOf<String?>(null) }

    LaunchedEffect(dniPersona) {
        withContext(Dispatchers.IO) {
            try {
                UnsafeSSL.ignoreSSLErrors()
                val gestor = GestorSQLExternModern()
                val url = "http://10.0.2.2/get_perfil.php?dni_persona=$dniPersona"
                val response = gestor.connectarObj(url)

                if (response == null) {
                    error = gestor.lastError ?: "No se recibió respuesta del servidor"
                } else if (response.has("error") && !response.isNull("error")) {
                    error = response.getString("error")
                } else {
                    val datosJson = response.optJSONObject("datos")

                    if (datosJson != null) {
                        val asigJson = response.optJSONArray("asignaturas") ?: org.json.JSONArray()
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
                                if (data.foto != null && data.foto != "Sin foto") {
                                    AsyncImage(
                                        model = data.foto,
                                        contentDescription = "Foto de perfil",
                                        modifier = Modifier
                                            .size(64.dp)
                                            .clip(CircleShape),
                                        contentScale = ContentScale.Crop
                                    )
                                } else {
                                    Icon(
                                        Icons.Default.Person,
                                        contentDescription = null,
                                        modifier = Modifier.size(64.dp)
                                    )
                                }
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

                    item {
                        Text(text = "Mis Asignaturas", style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold, modifier = Modifier.padding(vertical = 8.dp))
                    }

                    items(data.asignaturas) { asignatura ->
                        OutlinedCard(modifier = Modifier.fillMaxWidth()) {
                            Row(modifier = Modifier.padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
                                Icon(Icons.Default.Book, contentDescription = null, modifier = Modifier.size(20.dp), tint = MaterialTheme.colorScheme.secondary)
                                Spacer(modifier = Modifier.width(12.dp))
                                Text(text = asignatura, style = MaterialTheme.typography.bodyLarge)
                            }
                        }
                    }
                }
            }
        }
    }
}

