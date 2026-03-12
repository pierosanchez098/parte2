package com.example.loginframe

import android.content.Intent
import android.graphics.pdf.PdfDocument
import android.os.Bundle
import android.os.Environment
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.loginframe.components.AppDrawerScaffold
import com.example.loginframe.ui.theme.LoginFrameTheme
import com.example.loginframe.utils.GestorSQLExternModern
import com.example.loginframe.utils.UnsafeSSL
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject
import java.io.File
import java.io.FileOutputStream

class ViewBoletinActivity : ComponentActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        val prefs = getSharedPreferences("user_prefs", MODE_PRIVATE)
        val dniPersona = prefs.getString("dni_persona", "") ?: ""

        if (dniPersona.isEmpty()) {
            Toast.makeText(this, "Sessió no vàlida", Toast.LENGTH_LONG).show()
            startActivity(Intent(this, MainActivity::class.java))
            finish()
            return
        }

        setContent {
            LoginFrameTheme {
                AppDrawerScaffold(
                    currentScreenTitle = "Boletín de notas",
                    dniPersona = dniPersona
                ) { padding ->
                    BoletinScreen(
                        dniPersona = dniPersona,
                        modifier = Modifier.padding(padding)
                    )
                }
            }
        }
    }
}

@Composable
fun BoletinScreen(
    dniPersona: String,
    modifier: Modifier = Modifier
) {
    var notas by remember { mutableStateOf<List<NotaItem>>(emptyList()) }
    var curso by remember { mutableStateOf("Curso actual") }
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
            val url = "$baseUrl/get_boletin.php?dni_persona=$dniPersona"

            val gestor = GestorSQLExternModern()

            val jsonResponse: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObj(url)
            }

            android.util.Log.d("BOLETIN", "URL llamada: $url")
            android.util.Log.d("BOLETIN", "Respuesta JSON: ${jsonResponse?.toString() ?: "NULL"}")

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: "No se recibió respuesta del servidor"
            } else {
                if (jsonResponse.has("error") && !jsonResponse.isNull("error")) {
                    errorMessage = jsonResponse.getString("error")
                } else {
                    curso = jsonResponse.optString("curso", "Curso actual")

                    val jsonArray = jsonResponse.optJSONArray("notas") ?: JSONArray()

                    android.util.Log.d("BOLETIN", "Número de notas: ${jsonArray.length()}")

                    val list = mutableListOf<NotaItem>()
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        list.add(
                            NotaItem(
                                modul = obj.optString("modul", "Módulo desconocido"),
                                unitat = obj.optString("unitat", "UF desconocida"),
                                nota = obj.optString("nota", "Sin nota"),
                                dataNota = obj.optString("data_nota", "Sin fecha")
                            )
                        )
                    }
                    notas = list

                    if (notas.isEmpty()) {
                        errorMessage = "No hay notas registradas para este curso"
                    }
                }
            }
        } catch (e: Exception) {
            errorMessage = "Error al cargar: ${e.message}"
            android.util.Log.e("BOLETIN", "Excepción completa", e)
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
                text = "Boletín de notas",
                style = MaterialTheme.typography.headlineMedium
            )
            Text(
                text = "Curso: $curso",
                style = MaterialTheme.typography.titleMedium,
                color = MaterialTheme.colorScheme.primary
            )
        }

        when {
            isLoading -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
            }
            errorMessage != null -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text(
                        text = errorMessage!!,
                        color = MaterialTheme.colorScheme.error,
                        textAlign = TextAlign.Center,
                        modifier = Modifier.padding(24.dp)
                    )
                }
            }
            notas.isEmpty() -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text("No hay notas para este curso")
                }
            }
            else -> {
                LazyColumn(
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp)
                ) {
                    items(notas) { nota ->
                        Card(
                            modifier = Modifier.fillMaxWidth(),
                            elevation = CardDefaults.cardElevation(4.dp)
                        ) {
                            Column(modifier = Modifier.padding(16.dp)) {
                                Text(nota.modul, fontWeight = FontWeight.Bold, style = MaterialTheme.typography.titleMedium)
                                Text(nota.unitat, style = MaterialTheme.typography.bodyMedium)
                                Row(
                                    modifier = Modifier.fillMaxWidth(),
                                    horizontalArrangement = Arrangement.SpaceBetween
                                ) {
                                    Text("Nota: ${nota.nota}", fontWeight = FontWeight.Medium)
                                    Text(nota.dataNota, style = MaterialTheme.typography.bodySmall)
                                }
                            }
                        }
                    }
                }

                Button(
                    onClick = { generarPdf(notas, context, curso) },
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp)
                ) {
                    Text("Descargar PDF")
                }
            }
        }
    }
}

data class NotaItem(
    val modul: String,
    val unitat: String,
    val nota: String,
    val dataNota: String
)

private fun generarPdf(notas: List<NotaItem>, context: android.content.Context, curso: String) {
    try {
        val pdfDocument = PdfDocument()
        val pageInfo = PdfDocument.PageInfo.Builder(595, 842, 1).create()
        val page = pdfDocument.startPage(pageInfo)
        val canvas = page.canvas

        var y = 50f
        canvas.drawText("Boletín de Notas - $curso", 50f, y, android.graphics.Paint().apply { textSize = 18f })
        y += 40f

        for (nota in notas) {
            canvas.drawText("${nota.modul} - ${nota.unitat}: ${nota.nota}", 50f, y, android.graphics.Paint().apply { textSize = 14f })
            y += 30f
        }

        pdfDocument.finishPage(page)

        val file = File(
            Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS),
            "Boletin_Notas_$curso.pdf"
        )

        FileOutputStream(file).use { fos ->
            pdfDocument.writeTo(fos)
        }
        pdfDocument.close()

        Toast.makeText(context, "PDF guardado en Descargas", Toast.LENGTH_LONG).show()
    } catch (e: Exception) {
        Toast.makeText(context, "Error al generar PDF: ${e.message}", Toast.LENGTH_LONG).show()
    }
}