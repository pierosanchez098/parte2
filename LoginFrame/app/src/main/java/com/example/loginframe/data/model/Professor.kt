package com.example.loginframe.data.model

data class Professor(
    val nomComplet: String,
    val email: String,
    val foto: String?,
    val assignatures: List<String>
)