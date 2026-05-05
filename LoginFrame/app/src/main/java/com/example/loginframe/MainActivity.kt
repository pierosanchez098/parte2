package com.example.loginframe

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.input.ImeAction
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.data.GestorSQLExternModern
import com.example.data.SecureSessionManager
import com.example.data.UnsafeSSL
import com.example.feature_home.HomeActivity
import com.example.loginframe.ui.theme.LoginFrameTheme
import org.json.JSONObject
import java.net.URLEncoder
import kotlin.concurrent.thread

class MainActivity : ComponentActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val sessionManager = SecureSessionManager(this)
        val token = sessionManager.getToken()
        val dni = sessionManager.getDni()

        if (!token.isNullOrEmpty() && !dni.isNullOrEmpty()) {
            val intent = Intent(this, HomeActivity::class.java)
            startActivity(intent)
            finish()
            return
        }

        enableEdgeToEdge()
        setContent {
            LoginFrameTheme {
                LoginScreen()
            }
        }
    }
}

@Composable
fun LoginScreen() {
    var user by remember { mutableStateOf("") }
    var pass by remember { mutableStateOf("") }

    Box(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .align(Alignment.TopCenter),
            verticalArrangement = Arrangement.spacedBy(12.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text("Login")

            OutlinedTextField(
                value = user,
                onValueChange = { user = it },
                label = { Text("Usuario") },
                singleLine = true,
                keyboardOptions = KeyboardOptions(imeAction = ImeAction.Next),
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = pass,
                onValueChange = { pass = it },
                label = { Text("Contraseña") },
                singleLine = true,
                visualTransformation = PasswordVisualTransformation(),
                keyboardOptions = KeyboardOptions(
                    keyboardType = KeyboardType.Password,
                    imeAction = ImeAction.Done
                ),
                modifier = Modifier.fillMaxWidth()
            )

            LoginButton(
                user = user,
                pass = pass,
                modifier = Modifier.fillMaxWidth()
            )
        }
    }
}

@Composable
fun LoginButton(
    user: String,
    pass: String,
    modifier: Modifier = Modifier
) {
    val context = LocalContext.current

    Button(
        modifier = modifier,
        onClick = {
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
                        if (obj == null) {
                            val missatgeError = gestor.lastError ?: "Sin respuesta"
                            Toast.makeText(context, "Error: $missatgeError", Toast.LENGTH_LONG).show()
                        } else {
                            val potEntrar = obj.optBoolean("pot_entrar", false)

                            if (potEntrar) {
                                val token = obj.optString("token", "")
                                val dniPersona = obj.optString("dni_persona", "")

                                if (token.isNotEmpty() && dniPersona.isNotEmpty()) {

                                    val sessionManager = SecureSessionManager(context)
                                    sessionManager.saveSession(token, dniPersona)

                                    val intent = Intent(context, HomeActivity::class.java)
                                    context.startActivity(intent)
                                    (context as? MainActivity)?.finish()

                                } else {
                                    Toast.makeText(context, "Error: Token o DNI no recibidos", Toast.LENGTH_SHORT).show()
                                }
                            } else {
                                val errorMsg = obj.optString("tipus_derror", "Credenciales incorrectas")
                                Toast.makeText(context, errorMsg, Toast.LENGTH_SHORT).show()
                            }
                        }
                    }
                } catch (e: Exception) {
                    (context as? MainActivity)?.runOnUiThread {
                        Toast.makeText(context, "Error: ${e.message}", Toast.LENGTH_SHORT).show()
                    }
                }
            }
        }
    ) {
        Text("Entrar")
    }
}

@Preview(showBackground = true, showSystemUi = true)
@Composable
fun PreviewLoginScreen() {
    LoginFrameTheme {
        LoginScreen()
    }
}