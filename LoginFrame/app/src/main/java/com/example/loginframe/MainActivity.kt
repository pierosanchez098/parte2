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
import com.example.loginframe.ui.theme.LoginFrameTheme
import com.example.loginframe.utils.GestorSQLExternModern
import com.example.loginframe.utils.UnsafeSSL
import org.json.JSONObject
import java.net.URLEncoder
import kotlin.concurrent.thread

class MainActivity : ComponentActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
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
                label = { Text("Usuari") },
                singleLine = true,
                keyboardOptions = KeyboardOptions(imeAction = ImeAction.Next),
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = pass,
                onValueChange = { pass = it },
                label = { Text("Contrasenya") },
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
                            val missatgeError = gestor.lastError
                                ?: "Sense resposta o error desconegut"
                            Toast.makeText(context, "Error en la connexió: $missatgeError", Toast.LENGTH_LONG).show()
                        } else {
                            val potEntrar = obj.optBoolean("pot_entrar", false)

                            if (potEntrar) {
                                val dniPersona = obj.optString("dni_persona", "") ?: ""

                                if (dniPersona.isNotEmpty()) {
                                    val prefs = context.getSharedPreferences("user_prefs", android.content.Context.MODE_PRIVATE)
                                    prefs.edit().putString("dni_persona", dniPersona).apply()

                                    val intent = Intent(context, HomeActivity::class.java)
                                    intent.putExtra("DNI_PERSONA", dniPersona)
                                    context.startActivity(intent)
                                    (context as? MainActivity)?.finish()
                                } else {
                                    Toast.makeText(context, "No s'ha rebut l'identificador de l'usuari", Toast.LENGTH_SHORT).show()
                                }
                            } else {
                                val errorMsg = obj.optString("tipus_derror", "Usuari o contrasenya incorrectes")
                                Toast.makeText(context, errorMsg, Toast.LENGTH_SHORT).show()
                            }
                        }
                    }
                } catch (e: Exception) {
                    (context as? MainActivity)?.runOnUiThread {
                        Toast.makeText(context, "Excepció inesperada: ${e.message ?: "Desconeguda"}", Toast.LENGTH_SHORT).show()
                    }
                    e.printStackTrace()
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