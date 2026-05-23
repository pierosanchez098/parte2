package com.example.loginframe

import android.content.Intent
import android.widget.Toast
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Icon
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.OutlinedTextFieldDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
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
import com.example.data.GestorSQLExternModern
import com.example.data.SecureSessionManager
import com.example.data.UnsafeSSL
import com.example.loginframe.ui.theme.LoginFrameTheme
import org.json.JSONObject
import java.net.URLEncoder
import kotlin.concurrent.thread
import androidx.activity.ComponentActivity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import com.example.core.R
import com.example.core.utils.DeviceTracker

val Blue600 = Color(0xFF2563EB)
val Slate800 = Color(0xFF1E293B)
val Slate500 = Color(0xFF64748B)
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
            text = stringResource(id = R.string.login_welcome),
            fontSize = 32.sp,
            fontWeight = FontWeight.Bold,
            color = colorTitulo
        )
        Text(
            text = stringResource(id = R.string.login_subtitle),
            fontSize = 16.sp,
            color = colorSubtitulo,
            modifier = Modifier.padding(bottom = 40.dp)
        )

        OutlinedTextField(
            value = user,
            onValueChange = { user = it },
            label = { Text(stringResource(id = R.string.login_label_user)) },
            leadingIcon = { Icon(
                painter = painterResource(id = R.drawable.person_login_ic),
                contentDescription = null,
                tint = Blue600,
                modifier = Modifier.size(24.dp)
            ) },
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
            label = { Text(stringResource(id = R.string.login_label_password)) },
            leadingIcon = {
                Icon(
                    painter = painterResource(id = R.drawable.lock_ic),
                    contentDescription = null,
                    tint = Blue600,
                    modifier = Modifier.size(24.dp)
                )
            },
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
                    val baseUrl = "https://10.0.2.2"
                    val u = URLEncoder.encode(user, "UTF-8")
                    val p = URLEncoder.encode(pass, "UTF-8")

                    val tracker = DeviceTracker(context)
                    val userAgentHash = tracker.getUserAgentHash()
                    val uah = URLEncoder.encode(userAgentHash, "UTF-8")

                    val gestor = GestorSQLExternModern()

                    val obj: JSONObject? = gestor.connectarObjPOST(
                        "$baseUrl/login.php",
                        "user=$u&pass=$p&user_agent_hash=$uah"
                    )

                    (context as? ComponentActivity)?.runOnUiThread {
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

                                    val intent = Intent(context, MenuActivity::class.java).apply {
                                        putExtra("DNI_PERSONA", dniPersona)
                                    }
                                    context.startActivity(intent)

                                    (context as? ComponentActivity)?.finish()
                                }
                            } else {
                                val errorMsg = obj.optString("tipus_derror", "Credenciales incorrectas")
                                Toast.makeText(context, errorMsg, Toast.LENGTH_SHORT).show()
                            }
                        }
                    }
                } catch (e: Exception) {
                    (context as? ComponentActivity)?.runOnUiThread {
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
            Text(
                text = stringResource(id = R.string.login_btn_enter),
                fontSize = 18.sp,
                fontWeight = FontWeight.SemiBold
            )
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