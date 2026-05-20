package com.example.feature_home

import android.Manifest
import android.content.Context
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.platform.ComposeView
import androidx.core.content.ContextCompat
import androidx.fragment.app.Fragment
import androidx.work.ExistingPeriodicWorkPolicy
import androidx.work.PeriodicWorkRequestBuilder
import androidx.work.WorkManager
import java.util.concurrent.TimeUnit
import com.example.feature_home.utils.NotasCheckWorker

class HomeFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return ComposeView(requireContext()).apply {
            setContent {
                val context = requireContext()

                val launcherPermiso = rememberLauncherForActivityResult(
                    contract = ActivityResultContracts.RequestPermission()
                ) { isGranted ->
                    if (isGranted) {
                        activarDeteccionDeNotas(context)
                    } else {
                        Toast.makeText(context, "No recibirás alertas de nuevas notas.", Toast.LENGTH_SHORT).show()
                    }
                }

                LaunchedEffect(Unit) {
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
                        if (ContextCompat.checkSelfPermission(context, Manifest.permission.POST_NOTIFICATIONS) == PackageManager.PERMISSION_GRANTED) {
                            activarDeteccionDeNotas(context)
                        } else {
                            launcherPermiso.launch(Manifest.permission.POST_NOTIFICATIONS)
                        }
                    } else {
                        activarDeteccionDeNotas(context)
                    }
                }

                HomeScreen()
            }
        }
    }

    private fun activarDeteccionDeNotas(context: Context) {
        val notasCheckRequest = PeriodicWorkRequestBuilder<NotasCheckWorker>(15, TimeUnit.MINUTES)
            .build()

        WorkManager.getInstance(context).enqueueUniquePeriodicWork(
            "CheckNuevasNotasWork",
            ExistingPeriodicWorkPolicy.KEEP,
            notasCheckRequest
        )
    }
}