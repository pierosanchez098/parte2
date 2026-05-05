package com.example.feature_expediente

import android.content.Context
import android.content.Intent
import android.graphics.Color
import android.graphics.Paint
import android.graphics.pdf.PdfDocument
import android.os.Bundle
import android.os.Environment
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.data.GestorSQLExternModern
import com.example.data.UnsafeSSL
import com.example.domain.model.NotaItem
import com.example.core.utils.GestorTema
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

        val gestorTema = GestorTema(this)

        val prefs = getSharedPreferences("user_prefs", MODE_PRIVATE)
        val dniPersona = prefs.getString("dni_persona", "") ?: ""

        if (dniPersona.isEmpty()) {
            Toast.makeText(this, "Sesión no válida", Toast.LENGTH_LONG).show()
            val intent = Intent().setClassName(packageName, "com.example.loginframe.MainActivity")
            startActivity(intent)
            finish()
            return
        }

        setContent {

            var isDark by remember { mutableStateOf(gestorTema.isDarkMode()) }

            LoginFrameTheme(darkTheme = isDark) {
                AppDrawerScaffold(
                    currentScreenTitle = "Boletín de notas",
                    dniPersona = dniPersona,
                    isDarkMode = isDark,
                    onThemeChanged = { nuevoValor ->
                        isDark = nuevoValor
                        gestorTema.setDarkMode(nuevoValor)
                    }
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
            errorMessage = "No se ha recibido el identificador del usuario"
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

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: "No se recibió respuesta del servidor"
            } else {
                val errorValue = jsonResponse.optString("error", "")
                if (!jsonResponse.isNull("error") && errorValue.isNotBlank() && errorValue != "null") {
                    errorMessage = errorValue
                } else {
                    curso = jsonResponse.optString("curso", "Curso actual")
                    val jsonArray = jsonResponse.optJSONArray("notas") ?: JSONArray()
                    val list = mutableListOf<NotaItem>()
                    for (i in 0 until jsonArray.length()) {
                        val obj = jsonArray.getJSONObject(i)
                        list.add(NotaItem(
                            modul = obj.optString("modul", "Módulo"),
                            unitat = obj.optString("unitat", "UF"),
                            nota = obj.optString("nota", "-"),
                            dataNota = obj.optString("data_nota", "-")
                        ))
                    }
                    notas = list
                    if (notas.isEmpty()) errorMessage = "No hay notas registradas"
                }
            }
        } catch (e: Exception) {
            errorMessage = "Error: ${e.message}"
        } finally {
            isLoading = false
        }
    }

    Column(
        modifier = modifier
            .fillMaxSize()
            .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        if (isLoading) {
            CircularProgressIndicator()
        } else if (errorMessage != null) {
            Text(text = errorMessage!!, color = MaterialTheme.colorScheme.error, textAlign = TextAlign.Center)
        } else {
            Text(
                text = "Boletín de notas disponible",
                style = MaterialTheme.typography.headlineSmall,
                fontWeight = FontWeight.Bold
            )
            Text(
                text = "Curso escolar: $curso",
                style = MaterialTheme.typography.bodyLarge,
                color = MaterialTheme.colorScheme.secondary,
                modifier = Modifier.padding(top = 8.dp)
            )

            Spacer(modifier = Modifier.height(32.dp))

            Surface(
                modifier = Modifier.size(120.dp),
                color = MaterialTheme.colorScheme.primaryContainer,
                shape = MaterialTheme.shapes.medium
            ) {
                Box(contentAlignment = Alignment.Center) {
                    Text("PDF", style = MaterialTheme.typography.headlineLarge, color = MaterialTheme.colorScheme.onPrimaryContainer)
                }
            }

            Spacer(modifier = Modifier.height(32.dp))

            Button(
                onClick = { generarPdf(notas, context, curso) },
                modifier = Modifier.fillMaxWidth(),
                shape = MaterialTheme.shapes.medium
            ) {
                Text("Descargar Boletín en PDF")
            }

            Text(
                text = "El archivo se guardará en tu carpeta de Descargas",
                style = MaterialTheme.typography.bodySmall,
                modifier = Modifier.padding(top = 16.dp),
                color = MaterialTheme.colorScheme.outline
            )
        }
    }
}

private fun generarPdf(notas: List<NotaItem>, context: Context, curso: String) {
    try {
        val pdfDocument = PdfDocument()
        val pageInfo = PdfDocument.PageInfo.Builder(595, 842, 1).create()
        val page = pdfDocument.startPage(pageInfo)
        val canvas = page.canvas

        val titlePaint = Paint().apply {
            color = Color.BLACK
            textSize = 22f
            isFakeBoldText = true
        }

        val headerPaint = Paint().apply {
            color = Color.DKGRAY
            textSize = 14f
            isFakeBoldText = true
        }

        val bodyPaint = Paint().apply {
            color = Color.BLACK
            textSize = 12f
        }

        val linePaint = Paint().apply {
            color = Color.LTGRAY
            strokeWidth = 1f
            style = Paint.Style.STROKE
        }

        var y = 60f
        val margin = 50f
        val columnNotaX = 500f


        canvas.drawText("BOLETÍN", margin, y, titlePaint)
        y += 25f
        canvas.drawText("Curso escolar: $curso", margin, y, bodyPaint.apply { textSize = 14f })

        y += 20f
        canvas.drawLine(margin, y, 545f, y, linePaint.apply { strokeWidth = 2f })

        y += 40f

        canvas.drawText("MÓDULO / UNIDAD", margin, y, headerPaint)
        canvas.drawText("NOTA", columnNotaX, y, headerPaint)

        y += 10f
        canvas.drawLine(margin, y, 545f, y, linePaint.apply { strokeWidth = 1f })
        y += 25f

        bodyPaint.textSize = 12f
        for (nota in notas) {
            val textoModul = "${nota.modul} (${nota.unitat})"
            canvas.drawText(textoModul, margin, y, bodyPaint)

            canvas.drawText(nota.nota, columnNotaX, y, bodyPaint.apply { isFakeBoldText = true })
            bodyPaint.isFakeBoldText = false

            y += 15f
            canvas.drawLine(margin, y, 545f, y, linePaint)
            y += 25f

            if (y > 750f) break
        }

        y = 800f
        canvas.drawText("Documento generado automáticamente por el Sistema Académico.", margin, y, bodyPaint.apply {
            textSize = 10f
            color = Color.GRAY
        })

        pdfDocument.finishPage(page)

        val file = File(
            Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS),
            "Boletin_Notas_${curso.replace(" ", "_")}.pdf"
        )

        FileOutputStream(file).use { fos ->
            pdfDocument.writeTo(fos)
        }
        pdfDocument.close()

        Toast.makeText(context, "PDF guardado en Descargas", Toast.LENGTH_LONG).show()
    } catch (e: Exception) {
        Toast.makeText(context, "Error al generar PDF: ${e.message}", Toast.LENGTH_LONG).show()
        e.printStackTrace()
    }
}