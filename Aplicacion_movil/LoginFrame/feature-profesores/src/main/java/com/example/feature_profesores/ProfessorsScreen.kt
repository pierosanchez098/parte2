package com.example.feature_profesores

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Clear
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import coil.compose.AsyncImage
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.core.utils.GestorTema
import com.example.data.GestorSQLExternModern
import com.example.data.SecureSessionManager
import com.example.data.UnsafeSSL
import com.example.domain.model.Professor
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource

@Composable
fun ProfessorsScreen(
    dniPersona: String,
    modifier: Modifier = Modifier
) {
    var professors by remember { mutableStateOf<List<Professor>>(emptyList()) }
    var aula by remember { mutableStateOf<String?>(null) }
    var isLoading by remember { mutableStateOf(true) }
    var errorMessage by remember { mutableStateOf<String?>(null) }

    var textoBusqueda by remember { mutableStateOf("") }


    val profesoresFiltrados = remember(textoBusqueda, professors) {
        if (textoBusqueda.isEmpty()) {
            professors
        } else {
            professors.filter {
                it.nomComplet.contains(textoBusqueda, ignoreCase = true)
            }
        }
    }

    val context = LocalContext.current


    LaunchedEffect(dniPersona) {
        if (dniPersona.isEmpty()) {
            errorMessage = "No se ha recibido el identificador del usuario"
            isLoading = false
            return@LaunchedEffect
        }

        isLoading = true
        errorMessage = null

        try {
            UnsafeSSL.ignoreSSLErrors()

            val baseUrl = "http://10.0.2.2"
            val gestor = GestorSQLExternModern()

            val sessionManager = SecureSessionManager(context)
            val token = sessionManager.getToken() ?: ""

            val jsonResponse: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObjPOST("$baseUrl/get_professors.php", "token=$token&dni_persona=$dniPersona")
            }

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: context.getString(com.example.core.R.string.prof_err_no_server_response)
            } else {
                val newToken = jsonResponse.optString("new_token", "")
                if (newToken.isNotEmpty()) {
                    sessionManager.saveSession(newToken, dniPersona)
                }
                if (jsonResponse.optBoolean("expired", false)) {
                    sessionManager.logout()
                    val intent = Intent().apply {
                        setClassName(context.packageName, "com.example.loginframe.MainActivity")
                        flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                    }
                    context.startActivity(intent)
                    return@LaunchedEffect
                }

                if (jsonResponse.has("error") && !jsonResponse.isNull("error")) {
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
            }
        } catch (e: Exception) {
            errorMessage = "Error inesperado: ${e.message}"
            e.printStackTrace()
        } finally {
            isLoading = false
        }
    }

    Column(modifier = modifier.fillMaxSize()) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(id = com.example.core.R.string.prof_title),
                style = MaterialTheme.typography.headlineMedium
            )
            if (aula != null) {
                Text(
                    text = stringResource(id = com.example.core.R.string.prof_room, aula!!),
                    style = MaterialTheme.typography.titleMedium,
                    color = MaterialTheme.colorScheme.primary
                )
            } else {
                Text(
                    text = stringResource(id = com.example.core.R.string.prof_no_group),
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }

            Spacer(modifier = Modifier.height(16.dp))

            OutlinedTextField(
                value = textoBusqueda,
                onValueChange = { textoBusqueda = it },
                modifier = Modifier.fillMaxWidth(),
                placeholder = { Text(stringResource(id = com.example.core.R.string.prof_search_placeholder)) },
                leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                trailingIcon = {
                    if (textoBusqueda.isNotEmpty()) {
                        IconButton(onClick = { textoBusqueda = "" }) {
                            Icon(Icons.Default.Clear, contentDescription = stringResource(id = com.example.core.R.string.prof_clear_desc))
                        }
                    }
                },
                singleLine = true,
                shape = CircleShape
            )
        }

        when {
            isLoading -> {
                Box(modifier = Modifier.weight(1f).fillMaxWidth(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            }
            errorMessage != null -> {
                Box(modifier = Modifier.weight(1f).fillMaxWidth(), contentAlignment = Alignment.Center) {
                    Text("Error: $errorMessage", color = MaterialTheme.colorScheme.error)
                }
            }
            professors.isEmpty() -> {
                Box(modifier = Modifier.weight(1f).fillMaxWidth(), contentAlignment = Alignment.Center) {
                    Text(stringResource(id = com.example.core.R.string.prof_no_professors))
                }
            }
            else -> {
                LazyColumn(
                    modifier = Modifier.weight(1f),
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    items(profesoresFiltrados) { professor ->
                        ProfessorCard(professor = professor)
                    }

                    if (profesoresFiltrados.isEmpty() && textoBusqueda.isNotEmpty()) {
                        item {
                            Box(
                                modifier = Modifier.fillMaxWidth().padding(32.dp),
                                contentAlignment = Alignment.Center
                            ) {
                                Text(stringResource(id = com.example.core.R.string.prof_no_matches, textoBusqueda))
                            }
                        }
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
                contentDescription = stringResource(id = com.example.core.R.string.prof_img_desc, professor.nomComplet),
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
                    text = stringResource(id = com.example.core.R.string.prof_subjects_label),
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