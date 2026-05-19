package com.example.data

import android.content.Context
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey

class SecureSessionManager(context: Context) {

    private val masterKeyAlias = MasterKey.Builder(context)
        .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
        .build()

    private val sharedPreferences = EncryptedSharedPreferences.create(
        context,
        "secure_prefs",
        masterKeyAlias,
        EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
        EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
    )

    fun saveSession(token: String, dni: String) {
        sharedPreferences.edit().apply {
            putString("auth_token", token)
            putString("dni_persona", dni)
            putLong("last_interaction", System.currentTimeMillis())
            apply()
        }
    }

    fun getToken(): String? = sharedPreferences.getString("auth_token", null)
    fun getDni(): String? = sharedPreferences.getString("dni_persona", null)

    fun updateInteraction() {
        sharedPreferences.edit().putLong("last_interaction", System.currentTimeMillis()).apply()
    }

    fun clearSession() {
        sharedPreferences.edit().clear().apply()
    }

    fun logout() {
        sharedPreferences.edit().clear().apply()
    }

    fun saveToken(token: String) {
        sharedPreferences.edit().putString("auth_token", token).apply()
    }
}