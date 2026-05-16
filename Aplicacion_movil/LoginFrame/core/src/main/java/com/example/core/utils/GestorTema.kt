package com.example.core.utils
import android.content.Context
import android.content.SharedPreferences

class GestorTema (val context: Context) {
    private val prefs: SharedPreferences =
        context.getSharedPreferences("theme_prefs", Context.MODE_PRIVATE)

    fun setDarkMode(isDark: Boolean) {
        prefs.edit().putBoolean("is_dark_mode", isDark).apply()
    }

    fun isDarkMode(): Boolean {
        return prefs.getBoolean("is_dark_mode", false)
    }
}
