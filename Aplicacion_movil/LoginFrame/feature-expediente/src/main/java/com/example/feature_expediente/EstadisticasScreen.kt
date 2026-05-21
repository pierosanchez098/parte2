package com.example.feature_expediente

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.nativeCanvas
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.core.utils.Device
import com.example.core.utils.DeviceTracker
import com.example.core.utils.GestorTema
import com.example.data.GestorSQLExternModern
import com.example.data.SecureSessionManager
import com.example.data.UnsafeSSL
import com.example.domain.model.AsignaturaMedia
import com.example.domain.model.FctProgreso
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONObject
@Composable
fun EstadisticasScreen(
    dniPersona: String,
    modifier: Modifier = Modifier
) {
    var datosFct by remember { mutableStateOf<FctProgreso?>(null) }
    var listaMedias by remember { mutableStateOf<List<AsignaturaMedia>>(emptyList()) }

    var isLoading by remember { mutableStateOf(true) }
    var errorMessage by remember { mutableStateOf<String?>(null) }
    var pantallaActual by remember { mutableStateOf(0) }

    val context = LocalContext.current
    val sessionManager = remember { SecureSessionManager(context) }

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
            val token = sessionManager.getToken() ?: ""
            val tracker = DeviceTracker(context)
            val userAgentHash = tracker.getUserAgentHash()

            val responseFct: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObjPOST(
                    "$baseUrl/get_progres_fct.php",
                    "token=$token&dni_persona=$dniPersona&user_agent_hash=$userAgentHash"
                )
            }

            var tokenActualizado = token
            if (responseFct != null) {
                if (responseFct.optBoolean("expired", false)) {
                    val mensajeServidor = responseFct.optString("error", null)
                    withContext(Dispatchers.Main) {
                        Device(context, sessionManager).forzarReLogin(mensajeServidor)
                    }
                    return@LaunchedEffect
                }

                val newToken = responseFct.optString("new_token", "")
                if (newToken.isNotEmpty()) {
                    sessionManager.saveSession(newToken, dniPersona)
                    tokenActualizado = newToken
                }
            }

            val responseMedias: JSONObject? = withContext(Dispatchers.IO) {
                gestor.connectarObjPOST(
                    "$baseUrl/get_medias_asignaturas.php",
                    "token=$tokenActualizado&dni_persona=$dniPersona&user_agent_hash=$userAgentHash"
                )
            }

            if (responseMedias != null) {
                if (responseMedias.optBoolean("expired", false)) {
                    val mensajeServidor = responseMedias.optString("error", null)
                    withContext(Dispatchers.Main) {
                        Device(context, sessionManager).forzarReLogin(mensajeServidor)
                    }
                    return@LaunchedEffect
                }

                val newToken2 = responseMedias.optString("new_token", "")
                if (newToken2.isNotEmpty()) {
                    sessionManager.saveSession(newToken2, dniPersona)
                }
            }

            if (responseFct == null && responseMedias == null) {
                errorMessage = context.getString(com.example.core.R.string.stats_err_no_server_response)
            } else {
                if (responseFct != null) {
                    val errorFct = responseFct.optString("error", "")
                    if (responseFct.isNull("error") || errorFct.isBlank() || errorFct == "null") {
                        val fctObj = responseFct.optJSONObject("fct")
                        if (fctObj != null) {
                            datosFct = FctProgreso(
                                empresa = fctObj.optString("empresa", "Empresa no asignada"),
                                horasTotales = fctObj.optInt("horas_totales", 0),
                                horasHechas = fctObj.optInt("horas_hechas", 0),
                                estudio = fctObj.optString("estudio", "N/A")
                            )
                        }
                    }
                }

                if (responseMedias != null) {
                    val errorMedias = responseMedias.optString("error", "")
                    if (!responseMedias.isNull("error") && errorMedias.isNotBlank() && errorMedias != "null") {
                        if (datosFct == null) {
                            errorMessage = errorMedias
                        }
                    } else {
                        val jsonArray = responseMedias.optJSONArray("medias")
                        if (jsonArray != null) {
                            val list = mutableListOf<AsignaturaMedia>()
                            for (i in 0 until jsonArray.length()) {
                                val obj = jsonArray.getJSONObject(i)
                                list.add(AsignaturaMedia(
                                    nombre = obj.optString("asignatura", "Asignatura"),
                                    media = obj.optDouble("media", 0.0).toFloat()
                                ))
                            }
                            listaMedias = list
                        }
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
            modifier = Modifier.fillMaxWidth().padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(text = stringResource(id = com.example.core.R.string.stats_title), style = MaterialTheme.typography.headlineMedium)
            Spacer(modifier = Modifier.height(16.dp))

            TabRow(selectedTabIndex = pantallaActual, modifier = Modifier.fillMaxWidth()) {
                Tab(selected = pantallaActual == 0, onClick = { pantallaActual = 0 }, text = { Text(stringResource(id = com.example.core.R.string.stats_tab_fct)) })
                Tab(selected = pantallaActual == 1, onClick = { pantallaActual = 1 }, text = { Text(stringResource(id = com.example.core.R.string.stats_tab_performance)) })
            }
        }

        if (isLoading) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                CircularProgressIndicator()
            }
        } else if (errorMessage != null) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                Text("Error: $errorMessage", color = MaterialTheme.colorScheme.error)
            }
        } else {
            when (pantallaActual) {
                0 -> {
                    if (datosFct != null) {
                        GraficoProgresoFctCard(fct = datosFct!!)
                    } else {
                        Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                            Text(stringResource(id = com.example.core.R.string.stats_no_fct_data))
                        }
                    }
                }
                1 -> {
                    if (listaMedias.isNotEmpty()) {
                        LazyColumn(
                            modifier = Modifier.fillMaxSize(),
                            contentPadding = PaddingValues(16.dp),
                            verticalArrangement = Arrangement.spacedBy(16.dp)
                        ) {
                            items(listaMedias) { itemMedia ->
                                FilaBarraRendimiento(itemMedia)
                            }
                        }
                    } else {
                        Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                            Text(stringResource(id = com.example.core.R.string.stats_no_performance_data))
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun FilaBarraRendimiento(asignatura: AsignaturaMedia) {
    val colorBarras = MaterialTheme.colorScheme.primary

    Card(
        modifier = Modifier.fillMaxWidth(),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp).fillMaxWidth()) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = asignatura.nombre,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.SemiBold,
                    modifier = Modifier.weight(1f)
                )
                Text(
                    text = stringResource(id = com.example.core.R.string.stats_score, asignatura.media),
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    color = colorBarras
                )
            }

            Spacer(modifier = Modifier.height(12.dp))

            Canvas(modifier = Modifier.fillMaxWidth().height(24.dp)) {
                val anchoTotal = size.width
                val altoTotal = size.height
                val fraccionNota = (asignatura.media / 10f).coerceIn(0f, 1f)

                drawRect(
                    color = Color.LightGray.copy(alpha = 0.4f),
                    size = Size(anchoTotal, altoTotal)
                )
                drawRect(
                    color = colorBarras,
                    size = Size(anchoTotal * fraccionNota, altoTotal)
                )
            }
        }
    }
}

@Composable
fun GraficoProgresoFctCard(fct: FctProgreso) {
    val porcentaje = if (fct.horasTotales > 0) fct.horasHechas.toFloat() / fct.horasTotales.toFloat() else 0f
    val porcentajeTexto = "${(porcentaje * 100).toInt()}%"

    Card(
        modifier = Modifier.fillMaxWidth().padding(16.dp),
        elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
    ) {
        Column(
            modifier = Modifier.padding(24.dp).fillMaxWidth(),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(text = fct.empresa, style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.SemiBold)
            Text(
                text = stringResource(id = com.example.core.R.string.stats_study, fct.estudio),
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )

            Spacer(modifier = Modifier.height(32.dp))

            Box(contentAlignment = Alignment.Center, modifier = Modifier.size(200.dp)) {
                val colorAnilloFondo = MaterialTheme.colorScheme.surfaceVariant
                val colorAnilloProgreso = MaterialTheme.colorScheme.primary

                Canvas(modifier = Modifier.fillMaxSize()) {
                    drawArc(
                        color = colorAnilloFondo,
                        startAngle = 0f,
                        sweepAngle = 360f,
                        useCenter = false,
                        style = androidx.compose.ui.graphics.drawscope.Stroke(width = 24f)
                    )
                    drawArc(
                        color = colorAnilloProgreso,
                        startAngle = -90f,
                        sweepAngle = porcentaje * 360f,
                        useCenter = false,
                        style = androidx.compose.ui.graphics.drawscope.Stroke(width = 24f, cap = androidx.compose.ui.graphics.StrokeCap.Round)
                    )
                }
                Column(horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(text = porcentajeTexto, style = MaterialTheme.typography.headlineMedium, fontWeight = FontWeight.Bold)
                    Text(text = stringResource(id = com.example.core.R.string.stats_completed), style = MaterialTheme.typography.labelSmall, color = MaterialTheme.colorScheme.onSurfaceVariant)
                }
            }

            Spacer(modifier = Modifier.height(32.dp))

            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceAround) {
                Column(horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(text = "${fct.horasHechas}h", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
                    Text(text = stringResource(id = com.example.core.R.string.stats_hours_done), style = MaterialTheme.typography.bodySmall)
                }
                Column(horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(text = "${fct.horasTotales}h", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
                    Text(text = stringResource(id = com.example.core.R.string.stats_hours_required), style = MaterialTheme.typography.bodySmall)
                }
            }
        }
    }
}