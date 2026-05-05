package com.example.loginframe.data.model

data class PerfilData(
    val nombre: String,
    val apellidos: String,
    val nia: Int,
    val grupo: String,
    val aula: String,
    val foto: String?,
    val asignaturas: List<String>
)