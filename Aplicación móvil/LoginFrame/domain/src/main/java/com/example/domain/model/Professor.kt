package com.example.domain.model

data class Professor(
    val nomComplet: String,
    val email: String,
    val foto: String?,
    val assignatures: List<String>
)