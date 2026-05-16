package com.example.loginframe

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.ImeAction
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.core.utils.GestorTema
import com.example.data.GestorSQLExternModern
import com.example.data.SecureSessionManager
import com.example.data.UnsafeSSL
import com.example.feature_home.HomeActivity
import com.example.loginframe.ui.theme.LoginFrameTheme
import org.json.JSONObject
import java.net.URLEncoder
import kotlin.concurrent.thread

val Blue600 = Color(0xFF2563EB)
val Slate50 = Color(0xFFF8FAFC)
val Slate800 = Color(0xFF1E293B)
val Slate500 = Color(0xFF64748B)

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        val gestorTema = GestorTema(this)

        setContent {
            val isDark by remember { mutableStateOf(gestorTema.isDarkMode()) }

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

@Composable
fun LoginScreen(isDarkMode: Boolean) {
    var user by remember { mutableStateOf("") }
    var pass by remember { mutableStateOf("") }

    val colorTitulo = if (isDarkMode) Color.White else Slate800
    val colorSubtitulo = if (isDarkMode) Color.LightGray else Slate500

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(horizontal = 32.dp),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = "Bienvenido",
            fontSize = 32.sp,
            fontWeight = FontWeight.Bold,
            color = colorTitulo
        )
        Text(
            text = "Inicia sesión para continuar",
            fontSize = 16.sp,
            color = colorSubtitulo,
            modifier = Modifier.padding(bottom = 40.dp)
        )

        OutlinedTextField(
            value = user,
            onValueChange = { user = it },
            label = { Text("Usuario") },
            leadingIcon = { Icon(Icons.Default.Person, contentDescription = null, tint = Blue600) },
            singleLine = true,
            shape = RoundedCornerShape(12.dp),
            colors = OutlinedTextFieldDefaults.colors(
                focusedBorderColor = Blue600,
                unfocusedBorderColor = Color.LightGray
            ),
            keyboardOptions = KeyboardOptions(imeAction = ImeAction.Next),
            modifier = Modifier.fillMaxWidth()
        )

        Spacer(modifier = Modifier.height(16.dp))

        OutlinedTextField(
            value = pass,
            onValueChange = { pass = it },
            label = { Text("Contraseña") },
            leadingIcon = { Icon(Icons.Default.Lock, contentDescription = null, tint = Blue600) },
            singleLine = true,
            shape = RoundedCornerShape(12.dp),
            visualTransformation = PasswordVisualTransformation(),
            colors = OutlinedTextFieldDefaults.colors(
                focusedBorderColor = Blue600,
                unfocusedBorderColor = Color.LightGray
            ),
            keyboardOptions = KeyboardOptions(
                keyboardType = KeyboardType.Password,
                imeAction = ImeAction.Done
            ),
            modifier = Modifier.fillMaxWidth()
        )

        Spacer(modifier = Modifier.height(32.dp))

        LoginButton(
            user = user,
            pass = pass,
            modifier = Modifier
                .fillMaxWidth()
                .height(56.dp)
        )
    }
}

@Composable
fun LoginButton(
    user: String,
    pass: String,
    modifier: Modifier = Modifier
) {
    val context = LocalContext.current
    var isLoading by remember { mutableStateOf(false) }

    Button(
        modifier = modifier,
        enabled = !isLoading,
        shape = RoundedCornerShape(12.dp),
        colors = ButtonDefaults.buttonColors(containerColor = Blue600),
        onClick = {
            isLoading = true
            thread {
                try {
                    UnsafeSSL.ignoreSSLErrors()
                    val baseUrl = "http://10.0.2.2"
                    val u = URLEncoder.encode(user, "UTF-8")
                    val p = URLEncoder.encode(pass, "UTF-8")
                    val gestor = GestorSQLExternModern()

                    val obj: JSONObject? = gestor.connectarObjPOST(
                        "$baseUrl/login.php",
                        "user=$u&pass=$p"
                    )

                    (context as? MainActivity)?.runOnUiThread {
                        isLoading = false
                        if (obj == null) {
                            val missatgeError = gestor.lastError ?: "Sin respuesta"
                            Toast.makeText(context, "Error: $missatgeError", Toast.LENGTH_LONG).show()
                        } else {
                            val potEntrar = obj.optBoolean("pot_entrar", false)
                            if (potEntrar) {
                                val token = obj.optString("token", "")
                                val dniPersona = obj.optString("dni_persona", "")
                                if (token.isNotEmpty() && dniPersona.isNotEmpty()) {
                                    SecureSessionManager(context).saveSession(token, dniPersona)
                                    context.startActivity(Intent(context, HomeActivity::class.java))
                                    (context as? MainActivity)?.finish()
                                }
                            } else {
                                val errorMsg = obj.optString("tipus_derror", "Credenciales incorrectas")
                                Toast.makeText(context, errorMsg, Toast.LENGTH_SHORT).show()
                            }
                        }
                    }
                } catch (e: Exception) {
                    (context as? MainActivity)?.runOnUiThread {
                        isLoading = false
                        Toast.makeText(context, "Error: ${e.message}", Toast.LENGTH_SHORT).show()
                    }
                }
            }
        }
    ) {
        if (isLoading) {
            CircularProgressIndicator(color = Color.White, modifier = Modifier.size(24.dp))
        } else {
            Text("Entrar", fontSize = 18.sp, fontWeight = FontWeight.SemiBold)
        }
    }
}

@Preview(showBackground = true, showSystemUi = true)
@Composable
fun PreviewLoginScreen() {
    LoginFrameTheme {
        LoginScreen(isDarkMode = false)
    }
}