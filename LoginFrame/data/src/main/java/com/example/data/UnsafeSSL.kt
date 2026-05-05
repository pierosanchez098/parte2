package com.example.data

import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.HttpsURLConnection
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

// ATENCIÓ: això DESACTIVA la verificació SSL.
object UnsafeSSL{

    fun ignoreSSLErrors(){

        // TrustManager que accepta TOTS els certificats sense verificar res
        val trustAllCerts=arrayOf<TrustManager>(
            object:X509TrustManager{
                override fun checkClientTrusted(chain:Array<out X509Certificate>?,authType:String?){}

                override fun checkServerTrusted(chain:Array<out X509Certificate>?,authType:String?){}

                override fun getAcceptedIssuers():Array<X509Certificate>{
                    return arrayOf()
                }
            }
        )

        val sslContext=SSLContext.getInstance("SSL")
        sslContext.init(null,trustAllCerts,SecureRandom())

        // Fem que totes les connexions HTTPS de l'app facin servir aquest SSL "permisiu"
        HttpsURLConnection.setDefaultSSLSocketFactory(sslContext.socketFactory)

        // I desactivem la verificació del nom del host (hostname)
        HttpsURLConnection.setDefaultHostnameVerifier{_,_-> true }
    }
}
