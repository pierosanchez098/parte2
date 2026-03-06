package com.example.loginframe.utils

import org.json.JSONArray
import org.json.JSONObject
import java.io.BufferedReader
import java.io.InputStreamReader
import java.net.HttpURLConnection
import java.net.URL

class GestorSQLExternModern{

    // Guardem aquí l'últim error per poder-lo mostrar a la UI si cal
    var lastError:String?=null

    fun connectar(urlString:String):JSONArray?{
        val resultat=StringBuilder()
        lastError=null

        return try{
            val url=URL(urlString)
            val connection=url.openConnection() as HttpURLConnection
            connection.requestMethod="GET"
            connection.connectTimeout=5000
            connection.readTimeout=5000
            connection.connect()

            val responseCode=connection.responseCode
            if(responseCode==HttpURLConnection.HTTP_OK){
                val inputStream=connection.inputStream
                val reader=BufferedReader(InputStreamReader(inputStream))
                reader.use{
                    it.forEachLine{line->resultat.append(line)}
                }
                JSONArray(resultat.toString())
            }else{
                lastError="HTTP $responseCode (URL=$urlString)"
                null
            }
        }catch(e:Exception){
            e.printStackTrace()
            lastError="Excepció: ${e.javaClass.simpleName} -> ${e.message}"
            null
        }
    }

    // Aquesta versió serveix per a objectes JSON. És a dir,
    // respostes JSON de tipus:
    // {"pot_entrar":true}
    // per l'exemple del login
    fun connectarObj(urlString:String):JSONObject?{
        val resultat=StringBuilder()
        lastError=null

        return try{
            val url=URL(urlString)
            val connection=url.openConnection() as HttpURLConnection
            connection.requestMethod="GET"
            connection.connectTimeout=5000
            connection.readTimeout=5000
            connection.connect()

            val responseCode=connection.responseCode
            if(responseCode==HttpURLConnection.HTTP_OK){
                val inputStream=connection.inputStream
                val reader=BufferedReader(InputStreamReader(inputStream))
                reader.use{
                    it.forEachLine{line->resultat.append(line)}
                }
                try{
                    JSONObject(resultat.toString())
                }catch(e:Exception){
                    lastError="JSON invàlid des de $urlString -> ${e.message}"
                    null
                }
            }else{
                lastError="HTTP $responseCode (URL=$urlString)"
                null
            }

        }catch(e:Exception){
            e.printStackTrace()
            lastError="Excepció: ${e.javaClass.simpleName} -> ${e.message}"
            null
        }
    }

    // Versió per POST: enviem user i pass al body, format x-www-form-urlencoded
    fun connectarObjPOST(urlString:String,params:String):JSONObject?{
        val resultat=StringBuilder()
        lastError=null

        return try{
            val url=URL(urlString)
            val connection=url.openConnection() as HttpURLConnection
            connection.requestMethod="POST"
            connection.connectTimeout=5000
            connection.readTimeout=5000

            // Necessari per enviar cos (body) en un POST
            connection.doOutput=true
            connection.setRequestProperty(
                "Content-Type",
                "application/x-www-form-urlencoded; charset=UTF-8"
            )

            // Enviem el body del POST: user=...&pass=...
            connection.outputStream.use{os->
                os.write(params.toByteArray(Charsets.UTF_8))
                os.flush()
            }

            val responseCode=connection.responseCode
            if(responseCode==HttpURLConnection.HTTP_OK){
                val inputStream=connection.inputStream
                val reader=BufferedReader(InputStreamReader(inputStream))
                reader.use{
                    it.forEachLine{line->resultat.append(line)}
                }
                try{
                    JSONObject(resultat.toString())
                }catch(e:Exception){
                    lastError="JSON invàlid des de $urlString -> ${e.message}"
                    null
                }
            }else{
                lastError="HTTP $responseCode (URL=$urlString)"
                null
            }
        }catch(e:Exception){
            e.printStackTrace()
            lastError="Excepció: ${e.javaClass.simpleName} -> ${e.message}"
            null
        }
    }
}
