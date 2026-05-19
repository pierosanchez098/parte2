package com.example.feature_home.utils

import android.content.Context
import androidx.work.Worker
import androidx.work.WorkerParameters
import com.example.core.utils.NotificationAyuda
import com.example.data.SecureSessionManager
import com.example.data.UnsafeSSL
import org.json.JSONObject
import java.io.OutputStreamWriter
import java.net.HttpURLConnection
import java.net.URL
import java.net.URLEncoder

class NotasCheckWorker(val context: Context, workerParams: WorkerParameters) : Worker(context, workerParams) {

    override fun doWork(): Result {
        val sessionManager = SecureSessionManager(context)

        val dniPersona = sessionManager.getDni() ?: ""
        val tokenActual = sessionManager.getToken() ?: ""

        if (dniPersona.isEmpty() || tokenActual.isEmpty()) {
            return Result.failure()
        }

        try {

            UnsafeSSL.ignoreSSLErrors()

            val url = URL("http://10.0.2.2/get_ultima_nota.php")
            val conn = url.openConnection() as HttpURLConnection
            conn.requestMethod = "POST"
            conn.doOutput = true

            val data = "dni_persona=${URLEncoder.encode(dniPersona, "UTF-8")}&token=${URLEncoder.encode(tokenActual, "UTF-8")}"
            OutputStreamWriter(conn.outputStream).use { it.write(data) }

            val response = conn.inputStream.bufferedReader().use { it.readText() }
            val json = JSONObject(response)

            if (!json.isNull("error")) {
                return Result.failure()
            }

            val newToken = json.optString("new_token", "")
            if (newToken.isNotEmpty()) {
                sessionManager.saveToken(newToken)
            }

            if (!json.isNull("ultima_nota")) {
                val ultimaNotaObj = json.getJSONObject("ultima_nota")
                val serverDataNotaRaw = ultimaNotaObj.optString("data_nota_raw", "")
                val serverNota = ultimaNotaObj.optString("nota", "")
                val serverUf = ultimaNotaObj.optString("uf_id", "")

                val prefs = context.getSharedPreferences("NotasWorkerPrefs", Context.MODE_PRIVATE)

                val esPrimeraCarga = !prefs.contains("ultima_data_nota_notificada")
                val ultimaDataNotificada = prefs.getString("ultima_data_nota_notificada", "")

                if (esPrimeraCarga) {
                    prefs.edit().putString("ultima_data_nota_notificada", serverDataNotaRaw).apply()


                    return Result.success()
                }

                if (serverDataNotaRaw != ultimaDataNotificada && serverDataNotaRaw.isNotEmpty()) {

                    prefs.edit().putString("ultima_data_nota_notificada", serverDataNotaRaw).apply()

                    NotificationAyuda.showNewBoletinNotification(
                        context,
                        ", se detectó una nota: $serverNota ,"
                    )
                }
            }

            return Result.success()
        } catch (e: Exception) {
            e.printStackTrace()
            return Result.retry()
        }
    }
}