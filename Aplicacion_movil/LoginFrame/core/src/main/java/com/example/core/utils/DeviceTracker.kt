package com.example.core.utils

import android.content.Context

class DeviceTracker(private val context: Context) {

    fun getUserAgent(): String {
        return System.getProperty("http.agent") ?: "Android-App"
    }

    fun getUserAgentHash(): String {
        return getUserAgent().hashCode().toString()
    }

}