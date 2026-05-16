package com.example.core.utils

import android.graphics.Bitmap
import java.io.ByteArrayOutputStream

object ImageUtils {
    fun processBitmap(bitmap: Bitmap): Bitmap {
        val size = minOf(bitmap.width, bitmap.height)
        val xOffset = (bitmap.width - size) / 2
        val yOffset = (bitmap.height - size) / 2
        val squaredBitmap = Bitmap.createBitmap(bitmap, xOffset, yOffset, size, size)

        return Bitmap.createScaledBitmap(squaredBitmap, 800, 800, true)
    }

    fun bitmapToByteArray(bitmap: Bitmap): ByteArray {
        val stream = ByteArrayOutputStream()
        bitmap.compress(Bitmap.CompressFormat.JPEG, 80, stream)
        return stream.toByteArray()
    }
}