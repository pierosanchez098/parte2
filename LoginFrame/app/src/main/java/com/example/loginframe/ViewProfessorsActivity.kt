package com.example.loginframe

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import coil.compose.AsyncImage
import com.example.loginframe.data.model.Professor
import com.example.loginframe.ui.theme.LoginFrameTheme
import com.example.loginframe.utils.GestorSQLExternModern
import com.example.loginframe.utils.UnsafeSSL
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject

class ViewProfessorsActivity : ComponentActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        val dniPersona = intent.getStringExtra("DNI_PERSONA") ?: ""

        setContent {
            LoginFrameTheme {
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    ProfessorsScreen(dniPersona = dniPersona)
                }
            }
        }
    }
}

@Composable
fun ProfessorsScreen(
    dniPersona: String,
    modifier: Modifier = Modifier
) {
    var professors by remember { mutableStateOf<List<Professor>>(emptyList()) }
    var aula by remember { mutableStateOf<String?>(null) }  // ← nuevo estado para el aula
    var isLoading by remember { mutableStateOf(true) }
    var errorMessage by remember { mutableStateOf<String?>(null) }

    LaunchedEffect(dniPersona) {
        if (dniPersona.isEmpty()) {
            errorMessage = "No s'ha rebut identificador d'usuari"
            isLoading = false
            return@LaunchedEffect
        }

        isLoading = true
        errorMessage = null

        try {
            UnsafeSSL.ignoreSSLErrors()

            val baseUrl = "http://192.168.1.29"  // o 10.0.2.2 en emulador
            val url = "$baseUrl/get_professors.php?dni_persona=$dniPersona"

            val gestor = GestorSQLExternModern()

            val jsonResponse: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObj(url)
            }

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: "No se recibió respuesta del servidor"
            } else if (jsonResponse.has("error") && !jsonResponse.isNull("error")) {
                errorMessage = jsonResponse.getString("error")
            } else {
                aula = jsonResponse.optString("aula", null).takeIf { it?.isNotBlank() == true }

                val jsonArray = jsonResponse.optJSONArray("professors") ?: JSONArray()

                if (jsonArray.length() == 0) {
                    professors = emptyList()
                } else {
                    val list = mutableListOf<Professor>()
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        val nomComplet = obj.optString("nomComplet", "Sin nombre")
                        val email = obj.optString("email", "Sin email")
                        val foto = if (obj.isNull("foto") || obj.optString("foto").isBlank()) null else obj.optString("foto")

                        val assignArray = obj.optJSONArray("assignatures") ?: JSONArray()
                        val assignList = mutableListOf<String>()
                        for (j in 0 until assignArray.length()) {
                            assignList.add(assignArray.optString(j, ""))
                        }

                        list.add(Professor(nomComplet, email, foto, assignList))
                    }
                    professors = list
                }
            }
        } catch (e: Exception) {
            errorMessage = "Error inesperado: ${e.message}"
            e.printStackTrace()
        } finally {
            isLoading = false
        }
    }

    Column(modifier = modifier.fillMaxSize()) {
        // Cabecera con aula
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = "Els meus professors",
                style = MaterialTheme.typography.headlineMedium
            )
            if (aula != null) {
                Text(
                    text = "Aula: $aula",
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.primary
                )
            } else {
                Text(
                    text = "Grup no asignat",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }

        when {
            isLoading -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            }
            errorMessage != null -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text("Error: $errorMessage", color = MaterialTheme.colorScheme.error)
                }
            }
            professors.isEmpty() -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text("No hay profesores asignados")
                }
            }
            else -> {
                LazyColumn(
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    items(professors) { professor ->
                        ProfessorCard(professor = professor)
                    }
                }
            }
        }
    }
}

@Composable
fun ProfessorCard(professor: Professor) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            verticalAlignment = Alignment.CenterVertically,
            horizontalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            AsyncImage(
                model = professor.foto ?: "https://via.placeholder.com/80?text=Prof",
                contentDescription = "Foto de ${professor.nomComplet}",
                modifier = Modifier
                    .size(80.dp)
                    .clip(CircleShape),
                contentScale = ContentScale.Crop
            )

            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = professor.nomComplet,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.SemiBold
                )
                Text(
                    text = professor.email,
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )

                Spacer(modifier = Modifier.height(8.dp))

                Text(
                    text = "Assignatures:",
                    style = MaterialTheme.typography.labelLarge
                )

                professor.assignatures.forEach { assign ->
                    Text(
                        text = "• $assign",
                        style = MaterialTheme.typography.bodyMedium,
                        modifier = Modifier.padding(start = 8.dp)
                    )
                }
            }
        }
    }
}