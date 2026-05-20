package com.example.loginframe

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.View
import androidx.activity.compose.setContent
import androidx.appcompat.app.AppCompatActivity
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.viewinterop.AndroidView
import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentActivity
import androidx.fragment.app.FragmentContainerView
import com.example.core.AppDrawerScaffold
import com.example.feature_expediente.*
import com.example.feature_home.HomeFragment
import com.example.feature_perfil.PerfilFragment
import com.example.feature_profesores.ProfesoresFragment
import com.example.loginframe.ui.theme.LoginFrameTheme

class MenuActivity : AppCompatActivity() {


    override fun onCreate(savedInstanceState: Bundle?) {

        super.onCreate(savedInstanceState)


        val dniPersona = intent.getStringExtra("DNI_PERSONA") ?: ""



        setContent {

            var currentScreenTitle by remember { mutableStateOf("Inicio") }


            var fragmentActual by remember { mutableStateOf<Fragment>(HomeFragment()) }

            var isDarkMode by remember { mutableStateOf(false) }

            LoginFrameTheme(darkTheme = isDarkMode) {
                AppDrawerScaffold(

                    currentScreenTitle = currentScreenTitle,

                    dniPersona = dniPersona,
                    isDarkMode = isDarkMode,

                    onThemeChanged = { nuevoValor ->
                        isDarkMode = nuevoValor
                    },

                    onMenuOptionClicked = { titulo ->


                        currentScreenTitle = titulo

                        fragmentActual = when (titulo.trim()) {
                            "Inicio" -> HomeFragment()

                            "Mi Perfil" -> PerfilFragment().apply {
                                arguments = Bundle().apply { putString("DNI_PERSONA", dniPersona) }
                            }

                            "Boletín de notas" -> BoletinFragment().apply {
                                arguments = Bundle().apply { putString("DNI_PERSONA", dniPersona) }
                            }

                            "Mis profesores" -> ProfesoresFragment().apply {
                                arguments = Bundle().apply { putString("DNI_PERSONA", dniPersona) }
                            }

                            "Gráficos y estadísticas" -> EstadisticasFragment().apply {
                                arguments = Bundle().apply { putString("DNI_PERSONA", dniPersona) }
                            }

                            "Expediente" -> ExpedienteFragment().apply {
                                arguments = Bundle().apply { putString("DNI_PERSONA", dniPersona) }
                            }

                            else -> {
                                HomeFragment()
                            }
                        }

                    }

                ) { paddingValues ->


                    AndroidView(

                        factory = { context ->

                            FragmentContainerView(context).apply {

                                id = View.generateViewId()



                                supportFragmentManager.beginTransaction()

                                    .replace(this.id, fragmentActual)

                                    .commit()

                            }

                        },

                        update = { view ->

                            supportFragmentManager.beginTransaction()

                                .replace(view.id, fragmentActual)

                                .commit()

                        },

                        modifier = Modifier

                            .padding(paddingValues)

                            .fillMaxSize()

                    )

                }

            }

        }

    }
}

