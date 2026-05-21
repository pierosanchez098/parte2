package com.example.core.utils

import android.content.Context
import android.content.Intent
import android.widget.Toast
import com.example.data.SecureSessionManager

class Device(private val context: Context, private val sessionManager: SecureSessionManager) {


    fun forzarReLogin(mensajeError: String?) {
        sessionManager.logout()

        val textoToast = mensajeError ?: "Sesión inválida por motivos de seguridad."
        Toast.makeText(context, textoToast, Toast.LENGTH_LONG).show()

        val intent = Intent().apply {
            setClassName(context.packageName, "com.example.loginframe.MainActivity")
            flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        }
        context.startActivity(intent)
    }
}