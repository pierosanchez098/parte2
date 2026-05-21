package com.example.loginframe

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.ComposeView
import androidx.compose.ui.platform.LocalContext
import androidx.fragment.app.Fragment
import com.example.core.utils.GestorTema
import com.example.loginframe.ui.theme.LoginFrameTheme

class LoginFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return ComposeView(requireContext()).apply {
            setContent {
                val context = LocalContext.current
                val isDark = remember { leerPreferenciaTema(context) }

                LoginFrameTheme(darkTheme = isDark) {
                    Surface(
                        modifier = Modifier.fillMaxSize(),
                        color = MaterialTheme.colorScheme.background
                    ) {
                        LoginScreen(isDarkMode = isDark)
                    }
                }
            }
        }
    }

    private fun leerPreferenciaTema(context: Context): Boolean {
        val prefs = context.getSharedPreferences("AppConfigPrefs", Context.MODE_PRIVATE)
        return prefs.getBoolean("PREF_DARK_MODE", false)
    }
}