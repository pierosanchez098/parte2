package com.example.loginframe

import android.os.Bundle
import androidx.fragment.app.FragmentActivity
import com.example.loginframe.ui.theme.LoginFrameTheme

class MainActivity : FragmentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        if (savedInstanceState == null) {
            supportFragmentManager.beginTransaction()
                .replace(R.id.contenedor_login_fragment, LoginFragment())
                .commit()
        }
    }
}