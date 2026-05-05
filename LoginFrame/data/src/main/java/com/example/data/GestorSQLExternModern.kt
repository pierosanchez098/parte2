package com.example.data

import org.json.JSONArray
import org.json.JSONObject
import java.io.BufferedReader
import java.io.InputStreamReader
import java.net.HttpURLConnection
import java.net.URL
import java.net.URLEncoder

class GestorSQLExternModern{

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

    fun enviarPost(urlS: String, parametros: Map<String, String>): JSONObject? {
        return try {
            val url = URL(urlS)
            val conn = url.openConnection() as HttpURLConnection
            conn.requestMethod = "POST"
            conn.doOutput = true
            conn.doInput = true
            conn.connectTimeout = 5000

            val postData = parametros.map { "${it.key}=${URLEncoder.encode(it.value, "UTF-8")}" }
                .joinToString("&")

            conn.outputStream.use { os ->
                os.write(postData.toByteArray(Charsets.UTF_8))
            }

            val response = conn.inputStream.bufferedReader().use { it.readText() }
            JSONObject(response)
        } catch (e: Exception) {
            lastError = e.message
            null
        }
    }

    fun connectarObjPOST(urlString:String,params:String):JSONObject?{
        val resultat=StringBuilder()
        lastError=null

        return try{
            val url=URL(urlString)
            val connection=url.openConnection() as HttpURLConnection
            connection.requestMethod="POST"
            connection.connectTimeout=5000
            connection.readTimeout=5000

            connection.doOutput=true
            connection.setRequestProperty(
                "Content-Type",
                "application/x-www-form-urlencoded; charset=UTF-8"
            )

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