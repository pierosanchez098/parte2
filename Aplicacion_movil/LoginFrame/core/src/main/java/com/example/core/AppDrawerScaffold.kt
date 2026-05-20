package com.example.core

import androidx.fragment.app.Fragment
import android.content.Context
import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatDelegate
import androidx.compose.foundation.clickable
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
import androidx.compose.ui.unit.sp
import com.example.data.SecureSessionManager
import kotlinx.coroutines.launch
import kotlin.jvm.java
import androidx.core.content.edit
import androidx.core.os.LocaleListCompat
import java.util.Locale


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
                    text = stringResource(id = R.string.menu_title),
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
                    label = { Text(stringResource(id = R.string.menu_profile)) },
                    selected = currentScreenTitle == "Mi Perfil",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Mi Perfil")
                    }
                )

                NavigationDrawerItem(
                    label = { Text(stringResource(id = R.string.menu_stats)) },
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
                    label = { Text(stringResource(id = R.string.menu_home)) },
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
                    label = { Text(stringResource(id = R.string.menu_teachers)) },
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
                    label = { Text(stringResource(id = R.string.menu_report_card)) },
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
                    label = { Text(stringResource(id = R.string.menu_record)) },
                    selected = currentScreenTitle == "Expediente",
                    onClick = {
                        scope.launch { drawerState.close() }
                        onMenuOptionClicked("Expediente")
                    }
                )



                Spacer(modifier = Modifier.weight(1f))

                LanguageSelectorComponent()

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
                    label = { Text(stringResource(id = R.string.menu_theme)) },
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
                    label = { Text(stringResource(id = R.string.menu_logout)) },
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
                    title = {
                        val translatedTitle = when (currentScreenTitle.trim()) {
                            "Inicio" -> stringResource(id = R.string.menu_home)
                            "Mi Perfil" -> stringResource(id = R.string.menu_profile)
                            "Gráficos y estadísticas" -> stringResource(id = R.string.menu_stats)
                            "Mis profesores" -> stringResource(id = R.string.menu_teachers)
                            "Boletín de notas" -> stringResource(id = R.string.menu_report_card)
                            "Expediente" -> stringResource(id = R.string.menu_record)
                            else -> currentScreenTitle
                        }
                        Text(translatedTitle)
                    },
                    navigationIcon = {
                        IconButton(onClick = { scope.launch { drawerState.open() } }) {
                            Icon(painter = painterResource(id = R.drawable.menu_ic), contentDescription = null, modifier = Modifier.size(24.dp))
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
fun LanguageSelectorComponent() {
    val languages = listOf(
        Pair("Español", "es"),
        Pair("Català", "ca-ES"),
        Pair("English", "en-US"),
        Pair("Français", "fr-FR")
    )
    var expanded by remember { mutableStateOf(false) }

    val locales: androidx.core.os.LocaleListCompat = AppCompatDelegate.getApplicationLocales()

    val currentLocaleCode = if (!locales.isEmpty) {
        locales.get(0)?.language ?: "es"
    } else {
        "es"
    }

    val currentLanguageName = languages.find { it.second.startsWith(currentLocaleCode) }?.first ?: "Español"

    Column(modifier = Modifier.padding(horizontal = 28.dp, vertical = 8.dp)) {
        Text(
            text = stringResource(id = R.string.menu_languages),
            style = MaterialTheme.typography.labelMedium,
            color = MaterialTheme.colorScheme.onSurfaceVariant
        )
        Box {
            Row(
                verticalAlignment = Alignment.CenterVertically,
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { expanded = true }
                    .padding(vertical = 6.dp)
            ) {
                Icon(
                    painter = painterResource(id = R.drawable.idiomas_ic),
                    contentDescription = stringResource(id = R.string.lang_icon_desc),
                    modifier = Modifier.size(20.dp),
                    tint = MaterialTheme.colorScheme.onSurfaceVariant
                )

                Spacer(modifier = Modifier.width(12.dp))

                Text(
                    text = currentLanguageName,
                    fontSize = 15.sp,
                    style = MaterialTheme.typography.bodyLarge
                )
            }
            DropdownMenu(expanded = expanded, onDismissRequest = { expanded = false }) {
                languages.forEach { (name, code) ->
                    DropdownMenuItem(
                        text = { Text(name) },
                        onClick = {
                            expanded = false
                            val appLocale = androidx.core.os.LocaleListCompat.forLanguageTags(code)
                            AppCompatDelegate.setApplicationLocales(appLocale)
                        }
                    )
                }
            }
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
        title = { Text(text = stringResource(id = R.string.theme_dialog_title)) },
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
                    Text(
                        text = stringResource(id = R.string.theme_light_option),
                        modifier = Modifier.padding(start = 12.dp)
                    )
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
                    Text(
                        text = stringResource(id = R.string.theme_dark_option),
                        modifier = Modifier.padding(start = 12.dp)
                    )
                }
            }
        },
        confirmButton = {
            TextButton(onClick = { onConfirm(selectedIsDark) }) {
                Text(stringResource(id = R.string.theme_btn_accept))
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text(stringResource(id = R.string.theme_btn_cancel))
            }
        }
    )
}