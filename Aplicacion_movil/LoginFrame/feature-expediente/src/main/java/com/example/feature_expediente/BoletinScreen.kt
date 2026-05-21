package com.example.feature_expediente

import android.content.Context
import android.content.Intent
import androidx.compose.foundation.lazy.items
import android.graphics.Color
import android.graphics.Paint
import android.graphics.pdf.PdfDocument
import android.os.Bundle
import android.os.Environment
import android.os.Handler
import android.os.Looper
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.core.utils.DeviceTracker
import com.example.data.GestorSQLExternModern
import com.example.data.UnsafeSSL
import com.example.domain.model.NotaItem
import com.example.core.utils.GestorTema
import com.example.data.SecureSessionManager
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.json.JSONArray
import org.json.JSONObject
import java.io.File
import java.io.FileOutputStream

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

    val sessionManager = remember { SecureSessionManager(context) }
    val coroutineScope = rememberCoroutineScope()

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

            val token = sessionManager.getToken() ?: ""
            val tracker = DeviceTracker(context)
            val userAgentHash = tracker.getUserAgentHash()

            val jsonResponse: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObjPOST(
                    "$baseUrl/get_boletin.php",
                    "token=$token&dni_persona=$dniPersona&user_agent_hash=$userAgentHash"
                )
            }

            if (jsonResponse == null) {
                errorMessage = gestor.lastError ?: context.getString(com.example.core.R.string.report_err_no_server_response)
            } else {

                if (jsonResponse.optBoolean("expired", false)) {
                    sessionManager.logout()
                    val intent = Intent().apply {
                        setClassName(context.packageName, "com.example.loginframe.MainActivity")
                        flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                    }
                    context.startActivity(intent)
                    return@LaunchedEffect
                }

                val newToken = jsonResponse.optString("new_token", "")
                if (newToken.isNotEmpty()) {
                    sessionManager.saveSession(newToken, dniPersona)
                }

                val errorValue = jsonResponse.optString("error", "")
                if (!jsonResponse.isNull("error") && errorValue.isNotBlank() && errorValue != "null") {
                    errorMessage = errorValue
                } else {
                    curso = jsonResponse.optString("curso", "Curso escolar")
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
                    if (notas.isEmpty()) errorMessage = context.getString(com.example.core.R.string.report_err_no_grades)
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
                text = stringResource(id = com.example.core.R.string.report_title),
                style = MaterialTheme.typography.headlineSmall,
                fontWeight = FontWeight.Bold
            )
            Text(
                text = stringResource(id = com.example.core.R.string.report_school_year, curso),
                style = MaterialTheme.typography.bodyLarge,
                color = MaterialTheme.colorScheme.secondary,
                modifier = Modifier.padding(top = 8.dp)
            )

            Spacer(modifier = Modifier.height(32.dp))

            LazyColumn(
                modifier = Modifier
                    .weight(1f)
                    .fillMaxWidth(),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(notas) { item ->
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.4f))
                    ) {
                        Row(
                            modifier = Modifier.padding(16.dp).fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween,
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Column(modifier = Modifier.weight(1f)) {
                                Text(text = item.modul, style = MaterialTheme.typography.bodyLarge, fontWeight = FontWeight.Bold)
                                Text(text = stringResource(id = com.example.core.R.string.report_unit_label, item.unitat), style = MaterialTheme.typography.bodyMedium)
                                Text(text = stringResource(id = com.example.core.R.string.report_date_label, item.dataNota), style = MaterialTheme.typography.bodySmall, color = MaterialTheme.colorScheme.outline)
                            }
                            Text(
                                text = item.nota,
                                style = MaterialTheme.typography.titleLarge,
                                fontWeight = FontWeight.Bold,
                                color = MaterialTheme.colorScheme.primary
                            )
                        }
                    }
                }
            }

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
                onClick = {
                    coroutineScope.launch {
                        withContext(Dispatchers.IO) {
                            generarPdf(notas, context, curso)
                        }
                    }
                },
                modifier = Modifier.fillMaxWidth(),
                shape = MaterialTheme.shapes.medium
            ) {
                Text(stringResource(id = com.example.core.R.string.report_download_btn))
            }

            Text(
                text = stringResource(id = com.example.core.R.string.report_download_footer),
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
        val columnFechaX = 360f
        val columnNotaX = 500f

        canvas.drawText(context.getString(com.example.core.R.string.report_title).uppercase(), margin, y, titlePaint)
        y += 25f
        canvas.drawText(context.getString(com.example.core.R.string.report_school_year, curso), margin, y, bodyPaint.apply { textSize = 14f })

        y += 20f
        canvas.drawLine(margin, y, 545f, y, linePaint.apply { strokeWidth = 2f })

        y += 40f

        canvas.drawText(context.getString(com.example.core.R.string.report_pdf_header_subject), margin, y, headerPaint)
        canvas.drawText(context.getString(com.example.core.R.string.report_pdf_header_date), columnFechaX, y, headerPaint)
        canvas.drawText(context.getString(com.example.core.R.string.report_pdf_header_grade), columnNotaX, y, headerPaint)

        y += 10f
        canvas.drawLine(margin, y, 545f, y, linePaint.apply { strokeWidth = 1f })
        y += 25f

        bodyPaint.textSize = 12f
        bodyPaint.textSize = 12f

        val anchoMaximoTexto = (columnFechaX - margin - 20f).toInt()

        for (nota in notas) {
            val textoModul = "${nota.modul} (${nota.unitat})"

            val textLayout = if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M) {
                android.text.StaticLayout.Builder.obtain(textoModul, 0, textoModul.length, android.text.TextPaint(bodyPaint), anchoMaximoTexto)
                    .setAlignment(android.text.Layout.Alignment.ALIGN_NORMAL)
                    .setLineSpacing(0f, 1.0f)
                    .setIncludePad(false)
                    .build()
            } else {
                @Suppress("DEPRECATION")
                android.text.StaticLayout(
                    textoModul,
                    android.text.TextPaint(bodyPaint),
                    anchoMaximoTexto,
                    android.text.Layout.Alignment.ALIGN_NORMAL,
                    1.0f,
                    0f,
                    false
                )
            }

            val yAlineacionColumnas = y + 12f

            canvas.save()
            canvas.translate(margin, y)
            textLayout.draw(canvas)
            canvas.restore()

            canvas.drawText(nota.dataNota, columnFechaX, yAlineacionColumnas, bodyPaint)

            canvas.drawText(nota.nota, columnNotaX, yAlineacionColumnas, bodyPaint.apply { isFakeBoldText = true })
            bodyPaint.isFakeBoldText = false

            val alturaDelTexto = textLayout.height

            y += alturaDelTexto + 8f
            canvas.drawLine(margin, y, 545f, y, linePaint)
            y += 20f

            if (y > 750f) break
        }

        y = 800f
        canvas.drawText(context.getString(com.example.core.R.string.report_pdf_watermark), margin, y, bodyPaint.apply {
            textSize = 10f
            color = Color.GRAY
        })

        pdfDocument.finishPage(page)

        val nombreArchivo = "Boletin_Notas_${curso.replace(" ", "_")}.pdf"
        val resolver = context.contentResolver

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.Q) {
            val contentValues = android.content.ContentValues().apply {
                put(android.provider.MediaStore.MediaColumns.DISPLAY_NAME, nombreArchivo)
                put(android.provider.MediaStore.MediaColumns.MIME_TYPE, "application/pdf")
                put(android.provider.MediaStore.MediaColumns.RELATIVE_PATH, android.os.Environment.DIRECTORY_DOWNLOADS)
            }

            val uri = resolver.insert(android.provider.MediaStore.Downloads.EXTERNAL_CONTENT_URI, contentValues)
            if (uri != null) {
                resolver.openOutputStream(uri).use { outputStream ->
                    if (outputStream != null) {
                        pdfDocument.writeTo(outputStream)
                    } else {
                        throw java.io.IOException("No se pudo abrir el flujo de salida")
                    }
                }
            } else {
                throw java.io.IOException("No se pudo crear la entrada en MediaStore")
            }
        } else {
            val rutaDescargas = android.os.Environment.getExternalStoragePublicDirectory(android.os.Environment.DIRECTORY_DOWNLOADS)
            val archivo = java.io.File(rutaDescargas, nombreArchivo)
            java.io.FileOutputStream(archivo).use { fos ->
                pdfDocument.writeTo(fos)
            }
        }

        android.os.Handler(android.os.Looper.getMainLooper()).post {
            Toast.makeText(context, context.getString(com.example.core.R.string.report_toast_saved), Toast.LENGTH_LONG).show()
        }
    } catch (e: Exception) {
        android.os.Handler(android.os.Looper.getMainLooper()).post {
            Toast.makeText(context, context.getString(com.example.core.R.string.report_toast_error, e.message ?: ""), Toast.LENGTH_LONG).show()
        }
        e.printStackTrace()
    }
}