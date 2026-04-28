package com.example.loginframe.components

import android.content.Intent
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.selection.selectable
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import com.example.loginframe.HomeActivity
import com.example.loginframe.MainActivity
import com.example.loginframe.ViewBoletinActivity
import com.example.loginframe.ViewExpedienteActivity
import com.example.loginframe.ViewProfessorsActivity
import kotlinx.coroutines.launch

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun AppDrawerScaffold(
    currentScreenTitle: String,
    dniPersona: String? = null,
    isDarkMode: Boolean,
    onThemeChanged: (Boolean) -> Unit,
    content: @Composable (PaddingValues) -> Unit
) {
    val drawerState = rememberDrawerState(initialValue = DrawerValue.Closed)
    val scope = rememberCoroutineScope()
    val context = LocalContext.current

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
                    icon = { Icon(Icons.Filled.Home, contentDescription = null) },
                    label = { Text("Inicio") },
                    selected = currentScreenTitle == "Inicio",
                    onClick = {
                        val intent = Intent(context, HomeActivity::class.java)
                        context.startActivity(intent)
                        scope.launch { drawerState.close() }
                    }
                )

                NavigationDrawerItem(
                    icon = { Icon(Icons.Filled.School, contentDescription = null) },
                    label = { Text("Mis profesores") },
                    selected = currentScreenTitle == "Mis profesores",
                    onClick = {
                        val intent = Intent(context, ViewProfessorsActivity::class.java)
                        if (dniPersona != null) {
                            intent.putExtra("DNI_PERSONA", dniPersona)
                        }
                        context.startActivity(intent)
                        scope.launch { drawerState.close() }
                    }
                )

                NavigationDrawerItem(
                    icon = { Icon(Icons.Filled.Assessment, contentDescription = null) },
                    label = { Text("Boletín de notas") },
                    selected = currentScreenTitle == "Boletín de notas",
                    onClick = {
                        val intent = Intent(context, ViewBoletinActivity::class.java)
                        context.startActivity(intent)
                        scope.launch { drawerState.close() }
                    }
                )

                NavigationDrawerItem(
                    icon = { Icon(Icons.Filled.Assessment, contentDescription = null) },
                    label = { Text("Expediente") },
                    selected = currentScreenTitle == "Expediente",
                    onClick = {
                        val intent = Intent(context, ViewExpedienteActivity::class.java)
                        context.startActivity(intent)
                        scope.launch { drawerState.close() }
                    }
                )

                NavigationDrawerItem(
                    icon = { Icon(Icons.Filled.Schedule, contentDescription = null) },
                    label = { Text("Horario") },
                    selected = false,
                    onClick = {
                        scope.launch { drawerState.close() }
                    }
                )

                Spacer(modifier = Modifier.weight(1f))

                HorizontalDivider(modifier = Modifier.padding(vertical = 8.dp))

                NavigationDrawerItem(
                    icon = {
                        Icon(
                            imageVector = if (isDarkMode) Icons.Filled.DarkMode else Icons.Filled.LightMode,
                            contentDescription = null
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
                    icon = { Icon(Icons.Filled.Logout, contentDescription = null) },
                    label = { Text("Cerrar sesión") },
                    selected = false,
                    onClick = {
                        val intent = Intent(context, MainActivity::class.java)
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
                                imageVector = Icons.Filled.Menu,
                                contentDescription = "Abrir menú lateral"
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