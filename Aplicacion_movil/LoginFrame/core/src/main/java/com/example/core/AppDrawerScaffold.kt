package com.example.core

import androidx.fragment.app.Fragment
import android.content.Context
import android.content.Intent
import android.os.Bundle
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.selection.selectable
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.dp
import com.example.data.SecureSessionManager
import kotlinx.coroutines.launch
import kotlin.jvm.java
import androidx.core.content.edit


@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun AppDrawerScaffold(
    currentScreenTitle: String,
    dniPersona: String? = null,
    isDarkMode: Boolean,
    onThemeChanged: (Boolean) -> Unit,
    onMenuOptionClicked: (String) -> Unit,
    content: @Composable (PaddingValues) -> Unit
) {
    val drawerState = rememberDrawerState(initialValue = DrawerValue.Closed)
    val scope = rememberCoroutineScope()
    val context = LocalContext.current
    val packageName = context.packageName

    var showDialog by remember { mutableStateOf(false) }

    if (showDialog) {
        ThemeSelectionDialog(
            currentThemeIsDark = isDarkMode,
            onDismiss = { showDialog = false },
            onConfirm = { nuevoValor ->
                onThemeChanged(nuevoValor)
                showDialog = false
            }
        )
    }

    ModalNavigationDrawer(
        drawerState = drawerState,
        drawerContent = {
            ModalDrawerSheet {
                Spacer(modifier = Modifier.height(16.dp))
                Text(
                    text = "Menú",
                    modifier = Modifier.padding(horizontal = 28.dp, vertical = 16.dp),
                    style = MaterialTheme.typography.titleLarge
                )
                HorizontalDivider()


                NavigationDrawerItem(
                    icon = {
                        Icon(
                            painter = painterResource(id = R.drawable.profile_ic),
                            contentDescription = "Mi Perfil",
                            modifier = Modifier.size(24.dp)
                        )
                    },
                    label = { Text("Mi Perfil") },
                    selected = currentScreenTitle == "Mi Perfil",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Mi Perfil")
                    }
                )

                NavigationDrawerItem(
                    label = { Text("Estadísticas y Gráficos") },
                    selected = currentScreenTitle == "Gráficos y estadísticas",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Gráficos y estadísticas")
                    },
                    icon = {
                        Icon(
                            painter = painterResource(id = R.drawable.chart_ic),
                            contentDescription = "Gráficos y estadísticas",
                            modifier = Modifier.size(24.dp)
                        )
                    }
                )


                NavigationDrawerItem(
                    icon = {
                        Icon(
                            painter = painterResource(id = R.drawable.home_ic),
                            contentDescription = "Inicio",
                            modifier = Modifier.size(24.dp)
                        )
                    },
                    label = { Text("Inicio") },
                    selected = currentScreenTitle == "Inicio",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Inicio")
                    }
                )

                NavigationDrawerItem(
                    icon = {
                        Icon(
                            painter = painterResource(id = R.drawable.profes_ic),
                            contentDescription = "Mis profesores",
                            modifier = Modifier.size(24.dp)
                        )
                    },
                    label = { Text("Mis profesores") },
                    selected = currentScreenTitle == "Mis profesores",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Mis profesores")
                    }
                )

                NavigationDrawerItem(
                    icon = {
                        Icon(
                            painter = painterResource(id = R.drawable.check_ic),
                            contentDescription = "Mis notas",
                            modifier = Modifier.size(24.dp)
                        )
                    },
                    label = { Text("Boletín de notas") },
                    selected = currentScreenTitle == "Boletín de notas",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Boletín de notas")
                    }
                )

                NavigationDrawerItem(
                    icon = {
                        Icon(
                            painter = painterResource(id = R.drawable.libros_ic),
                            contentDescription = "Expediente",
                            modifier = Modifier.size(24.dp)
                        )
                    },
                    label = { Text("Expediente") },
                    selected = currentScreenTitle == "Expediente",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Expediente")
                    }
                )



                Spacer(modifier = Modifier.weight(1f))

                HorizontalDivider(modifier = Modifier.padding(vertical = 8.dp))

                NavigationDrawerItem(
                    icon = {
                        Icon(
                            painter = painterResource(
                                id = if (isDarkMode) R.drawable.moon_ic else R.drawable.sun_ic
                            ),
                            contentDescription = "Configurar Tema",
                            modifier = Modifier.size(24.dp)
                        )
                    },
                    label = { Text("Configurar Tema") },
                    selected = false,
                    onClick = {
                        scope.launch { drawerState.close() }
                        showDialog = true
                    }
                )



                NavigationDrawerItem(
                    icon = { Icon(
                        painter = painterResource(id = R.drawable.exit_ic),
                        contentDescription = "Salir",
                        modifier = Modifier.size(24.dp)
                    ) },
                    label = { Text("Cerrar sesión") },
                    selected = false,
                    onClick = {
                        val sessionManager = SecureSessionManager(context)
                        sessionManager.logout()

                        context.getSharedPreferences("NotasWorkerPrefs", Context.MODE_PRIVATE).edit { clear() }

                        val intent = Intent()
                        intent.setClassName(packageName, "com.example.loginframe.MainActivity")
                        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK

                        context.startActivity(intent)

                        scope.launch { drawerState.close() }
                    },
                    colors = NavigationDrawerItemDefaults.colors(
                        selectedContainerColor = MaterialTheme.colorScheme.errorContainer,
                        selectedIconColor = MaterialTheme.colorScheme.onErrorContainer,
                        selectedTextColor = MaterialTheme.colorScheme.onErrorContainer
                    )
                )
            }
        }
    ) {
        Scaffold(
            topBar = {
                TopAppBar(
                    title = { Text(currentScreenTitle) },
                    navigationIcon = {
                        IconButton(onClick = { scope.launch { drawerState.open() } }) {
                            Icon(
                                painter = painterResource(id = R.drawable.menu_ic),
                                contentDescription = "Abrir menú lateral",
                                modifier = Modifier.size(24.dp)
                            )
                        }
                    }
                )
            }
        ) { innerPadding ->
            content(innerPadding)
        }
    }
}



@Composable
fun ThemeSelectionDialog(
    currentThemeIsDark: Boolean,
    onDismiss: () -> Unit,
    onConfirm: (Boolean) -> Unit
) {
    var selectedIsDark by remember { mutableStateOf(currentThemeIsDark) }

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(text = "Configurar tema") },
        text = {
            Column(modifier = Modifier.fillMaxWidth()) {
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    modifier = Modifier
                        .fillMaxWidth()
                        .selectable(
                            selected = !selectedIsDark,
                            onClick = { selectedIsDark = false }
                        )
                        .padding(vertical = 8.dp)
                ) {
                    RadioButton(
                        selected = !selectedIsDark,
                        onClick = { selectedIsDark = false }
                    )
                    Text(text = "Tema claro", modifier = Modifier.padding(start = 12.dp))
                }

                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    modifier = Modifier
                        .fillMaxWidth()
                        .selectable(
                            selected = selectedIsDark,
                            onClick = { selectedIsDark = true }
                        )
                        .padding(vertical = 8.dp)
                ) {
                    RadioButton(
                        selected = selectedIsDark,
                        onClick = { selectedIsDark = true }
                    )
                    Text(text = "Tema oscuro", modifier = Modifier.padding(start = 12.dp))
                }
            }
        },
        confirmButton = {
            TextButton(onClick = { onConfirm(selectedIsDark) }) {
                Text("Aceptar")
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("Cancelar")
            }
        }
    )
}