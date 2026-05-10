package com.example.feature_home

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Text
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import com.example.core.AppDrawerScaffold
import com.example.core.theme.LoginFrameTheme
import com.example.core.utils.GestorTema

class HomeActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val gestorTema = GestorTema(this)

        setContent {

            var isDark by remember { mutableStateOf(gestorTema.isDarkMode()) }

            LoginFrameTheme (darkTheme = isDark){

                AppDrawerScaffold(
                    currentScreenTitle = "Inicio",
                    isDarkMode = isDark,
                    onThemeChanged = { nuevoValor ->
                        isDark = nuevoValor
                        gestorTema.setDarkMode(nuevoValor)
                    }
                ) { padding ->
                    Box(
                        modifier = Modifier
                            .fillMaxSize()
                            .padding(padding),
                        contentAlignment = Alignment.Center
                    ) {
                        Text("Has iniciado sesión correctamente.")
                    }
                }
            }
        }
    }
}