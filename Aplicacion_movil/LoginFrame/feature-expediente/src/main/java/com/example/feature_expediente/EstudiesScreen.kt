package com.example.feature_expediente

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CircularProgressIndicator
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
import com.example.data.GestorSQLExternModern
import com.example.data.UnsafeSSL
import com.example.domain.model.Estudis
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.core.utils.GestorTema
import com.example.data.SecureSessionManager
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject
import kotlin.jvm.java
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource

@Composable
fun EstudiesScreen(
    dniPersona: String,
    modifier: Modifier = Modifier
){
    var estudis by remember { mutableStateOf<List<Estudis>>(emptyList()) }
    var carnet by remember { mutableStateOf<String?>(null) }
    var isLoading by remember { mutableStateOf(true) }
    var errorMessage by remember { mutableStateOf<String?>(null) }

    val context = LocalContext.current

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

            val baseUrl = "http://10.0.2.2"
            val gestor = GestorSQLExternModern()

            val sessionManager = SecureSessionManager(context)
            val token = sessionManager.getToken() ?: ""

            val jsonResponse: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObjPOST("$baseUrl/get_estudis.php", "token=$token&dni_persona=$dniPersona")
            }

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: context.getString(com.example.core.R.string.studies_err_no_server_response)
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
                    carnet = jsonResponse.optString("foto_carnet", null).takeIf { it?.isNotBlank() == true }
                    val jsonArray = jsonResponse.optJSONArray("estudis") ?: JSONArray()

                    val list = mutableListOf<Estudis>()
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        val nomEstudi = obj.optString("nom_estudi", "No hay nombre de estudio")
                        val cursoInicio = obj.optString("curs_inici", "Sin fecha de inicio")
                        val cursoFin = obj.optString("curs_fi", "Sin fecha de fin")
                        val status = obj.optString("status", "Sin estado")
                        val notaFinal = obj.optString("nota_final", "Sin nota final")

                        list.add(Estudis(nomEstudi, cursoInicio, cursoFin, status, notaFinal))
                    }
                    estudis = list
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
                text = stringResource(id = com.example.core.R.string.studies_title),
                style = MaterialTheme.typography.headlineMedium
            )

            Spacer(modifier = Modifier.height(16.dp))

            if (carnet != null) {
                AsyncImage(
                    model = carnet ?: "https://via.placeholder.com/300x200?text=Sin+foto",
                    contentDescription = stringResource(id = com.example.core.R.string.studies_img_desc),
                    modifier = Modifier
                        .size(width = 280.dp, height = 200.dp)
                        .clip(RoundedCornerShape(12.dp))
                        .border(2.dp, MaterialTheme.colorScheme.primary, RoundedCornerShape(12.dp)),
                    contentScale = ContentScale.Crop
                )
            } else {
                Text(
                    text = stringResource(id = com.example.core.R.string.studies_no_img),
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
            estudis.isEmpty() -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text(stringResource(id = com.example.core.R.string.studies_empty_list))
                }
            }
            else -> {
                LazyColumn(
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    items(estudis) { estudis ->
                        EstudisCard(estudis)
                    }
                }
            }
        }
    }
}

@Composable
fun EstudisCard(estudis: Estudis) {
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

            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = estudis.nom_estudi,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.SemiBold
                )
                Text(
                    text = estudis.curs_inici,
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Text(
                    text = estudis.curs_fi.toString(),
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )

                Spacer(modifier = Modifier.height(8.dp))

                Text(
                    text = estudis.status,
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    fontWeight = FontWeight.Bold
                )

                Text(
                    text = estudis.nota_final.toString(),
                    style = MaterialTheme.typography.headlineMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    fontWeight = FontWeight.Bold
                )
            }
        }
    }
}