package com.example.core.utils

import android.graphics.Bitmap
import java.io.ByteArrayOutputStream
import android.content.ContentValues
import android.content.Context
import android.net.Uri
import android.provider.MediaStore
import android.util.Base64
import android.util.Log
import com.example.data.GestorSQLExternModern
import java.io.File
import java.io.FileOutputStream

object ImageUtils {
    fun procesarImagen(bitmap: Bitmap): Bitmap {
        val tamanyo = minOf(bitmap.width, bitmap.height)
        val x = (bitmap.width - tamanyo) / 2
        val y = (bitmap.height - tamanyo) / 2
        val cuadrado = Bitmap.createBitmap(bitmap, x, y, tamanyo, tamanyo)
        return Bitmap.createScaledBitmap(cuadrado, 512, 512, true)
    }

    fun comprimirImagen(bitmap: Bitmap): ByteArray {
        val flujoSalida = ByteArrayOutputStream()
        bitmap.compress(Bitmap.CompressFormat.JPEG, 75, flujoSalida)
        return flujoSalida.toByteArray()
    }

    fun guardarEnGaleria(contexto: Context, bitmap: Bitmap) {
        val valores = ContentValues().apply {
            put(MediaStore.Images.Media.DISPLAY_NAME, "perfil_${System.currentTimeMillis()}.jpg")
            put(MediaStore.Images.Media.MIME_TYPE, "image/jpeg")
            put(MediaStore.Images.Media.RELATIVE_PATH, "Pictures/LoginFrame")
        }

        val uri = contexto.contentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, valores)
        uri?.let { direccion ->
            contexto.contentResolver.openOutputStream(direccion).use { salida ->
                if (salida != null) {
                    bitmap.compress(Bitmap.CompressFormat.JPEG, 90, salida)
                }
            }
        }
    }

    fun obtenerUriDeImagen(context: Context, bitmap: Bitmap): Uri {
        val file = File(context.cacheDir, "temp_perfil.jpg")
        FileOutputStream(file).use { bitmap.compress(Bitmap.CompressFormat.JPEG, 100, it) }
        return Uri.fromFile(file)
    }


    fun subirFotoAlServidor(dni: String, bytes: ByteArray): Boolean {
        return try {
            val base64 = Base64.encodeToString(bytes, Base64.DEFAULT)
            val gestor = GestorSQLExternModern()

            val respuesta = gestor.enviarPost("http://10.0.2.2/subir_foto.php", mapOf("dni" to dni, "imagen" to base64))

            respuesta != null && gestor.lastError == null
        } catch (e: Exception) {
            Log.e("API", "Error subiendo: ${e.message}")
            false
        }
    }
}