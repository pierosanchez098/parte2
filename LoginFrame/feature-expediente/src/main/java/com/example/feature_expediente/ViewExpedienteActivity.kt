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
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject
import kotlin.jvm.java

class ViewExpedienteActivity : ComponentActivity() {
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
            val intent = Intent().setClassName(packageName, "com.example.loginframe.MainActivity")
            startActivity(intent)
            finish()
            return
        }

        setContent {

            var isDark by remember { mutableStateOf(gestorTema.isDarkMode()) }

            LoginFrameTheme(darkTheme = isDark) {
                AppDrawerScaffold(
                    currentScreenTitle = "Expediente de estudios finalizados",
                    dniPersona = dniPersona,
                    isDarkMode = isDark,
                    onThemeChanged = { nuevoValor ->
                        isDark = nuevoValor
                        gestorTema.setDarkMode(nuevoValor)
                    }
                ) { padding ->
                    EstudiesScreen(
                        dniPersona = dniPersona,
                        modifier = Modifier.padding(padding)
                    )
                }
            }
        }
    }
    }

@Composable
fun EstudiesScreen(
    dniPersona: String,
    modifier: Modifier = Modifier
){
    var estudis by remember { mutableStateOf<List<Estudis>>(emptyList()) }
    var carnet by remember { mutableStateOf<String?>(null) }
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

            val baseUrl = "http://10.0.2.2"
            val url = "$baseUrl/get_estudis.php?dni_persona=$dniPersona"

            val gestor = GestorSQLExternModern()

            val jsonResponse: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObj(url)
            }

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: "No se recibió respuesta del servidor"
            } else if (jsonResponse.has("error") && !jsonResponse.isNull("error")) {
                errorMessage = jsonResponse.getString("error")
            } else {

                carnet = jsonResponse.optString("foto_carnet", null).takeIf { it?.isNotBlank() == true }
                val jsonArray = jsonResponse.optJSONArray("estudis") ?: JSONArray()

                if (jsonArray.length() == 0) {
                    estudis = emptyList()
                } else {

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
            }catch (e: Exception) {
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
                text = "Estudios finalizados",
                style = MaterialTheme.typography.headlineMedium
            )

            Spacer(modifier = Modifier.height(16.dp))

            if (carnet != null) {
                AsyncImage(
                    model = carnet ?: "https://via.placeholder.com/300x200?text=Sin+foto",
                    contentDescription = "Foto del carnet del alumno",
                    modifier = Modifier
                        .size(width = 280.dp, height = 200.dp)
                        .clip(RoundedCornerShape(12.dp))
                        .border(2.dp, MaterialTheme.colorScheme.primary, RoundedCornerShape(12.dp)),
                    contentScale = ContentScale.Crop
                )
            } else {
                Text(
                    text = "Sin foto carnet",
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
                    Text("No hay estudios disponibles")
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