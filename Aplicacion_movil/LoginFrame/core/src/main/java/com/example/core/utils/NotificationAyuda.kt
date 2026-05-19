package com.example.core.utils

import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.content.Context
import android.content.Intent
import android.graphics.BitmapFactory
import android.os.Build
import androidx.core.app.NotificationCompat
import androidx.core.app.NotificationManagerCompat
import com.example.core.R

object NotificationAyuda {

    const val CHANNEL_ID = "boletin_notas_channel_v2"
    const val NOTIFICATION_ID = 1001

    fun createNotificationChannel(context: Context) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val name = "Boletín de Notas"
            val descriptionText = "Notificaciones cuando hay nuevas notas"

            val importance = NotificationManager.IMPORTANCE_HIGH

            val channel = NotificationChannel(CHANNEL_ID, name, importance).apply {
                description = descriptionText
                enableLights(true)
                enableVibration(true)
                lockscreenVisibility = NotificationCompat.VISIBILITY_PUBLIC
            }

            val notificationManager = context.applicationContext.getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
            notificationManager.createNotificationChannel(channel)
        }
    }

    fun showNewBoletinNotification(context: Context, curso: String = "2025-2026") {
        val appContext = context.applicationContext

        createNotificationChannel(appContext)

        val intent = Intent().apply {
            setClassName(appContext.packageName, "com.example.feature_expediente.ViewBoletinActivity")
            flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        }

        val pendingIntent = PendingIntent.getActivity(
            appContext,
            0,
            intent,
            PendingIntent.FLAG_IMMUTABLE or PendingIntent.FLAG_UPDATE_CURRENT
        )

        val logoColorBitmap = try {
            BitmapFactory.decodeResource(appContext.resources, R.drawable.logo_evalis_color)
        } catch (e: Exception) {
            null
        }

        val builder = NotificationCompat.Builder(appContext, CHANNEL_ID)
            .setSmallIcon(R.drawable.ic_evalis_silueta)
            .setContentTitle("Nuevo boletín de notas disponible")
            .setContentText("Se ha detectado una actualización de notas en el boletín.")
            .setPriority(NotificationCompat.PRIORITY_MAX)
            .setVibrate(longArrayOf(1000, 1000, 1000))
            .setAutoCancel(true)
            .setContentIntent(pendingIntent)

        if (logoColorBitmap != null) {
            builder.setLargeIcon(logoColorBitmap)
        }

        try {
            val notificationManager = appContext.getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager

            val idUnico = System.currentTimeMillis().toInt()
            notificationManager.notify(idUnico, builder.build())

        } catch (e: Exception) {
            e.printStackTrace()
        }
    }
}