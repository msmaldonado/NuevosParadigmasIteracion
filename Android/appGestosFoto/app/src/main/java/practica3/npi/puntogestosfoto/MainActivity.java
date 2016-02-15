package practica3.npi.puntogestosfoto;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

// Necesarias para manejar el gesto
import haibison.android.lockpattern.LockPatternActivity;
import haibison.android.lockpattern.utils.AlpSettings;

public class MainActivity extends Activity {
    private static final int CREAR_PATRON = 1;
    private static final int INTRODUCIR_PATRON = 2;
    private Button gestorButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        //Lanzamos el entorno de trabajo de activity_main
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        gestorButton = (Button) findViewById(R.id.botonIntroducirGesto);
        // Guardardamos el ultimo patrón creado
        AlpSettings.Security.setAutoSavePattern(this, true);

        // Al hacer click en el boton, tendremos que crear el patron
        Button botonCrearGesto = (Button) findViewById(R.id.botonCrearGesto);
        botonCrearGesto.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Necesitamos un Intent para crear el patron
                LockPatternActivity.IntentBuilder
                        .newPatternCreator(MainActivity.this)
                        .startForResult(MainActivity.this, CREAR_PATRON);

                gestorButton.setEnabled(true);
            }
        });


        // Al hacer click en el boton, tendremos que confirmar el patron
        Button botonIntroducirGesto = (Button) findViewById(R.id.botonIntroducirGesto);
        botonIntroducirGesto.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Necesitamos un Intent para confirmar el patron
                LockPatternActivity.IntentBuilder
                        .newPatternComparator(MainActivity.this)
                        .startForResult(MainActivity.this, INTRODUCIR_PATRON);
            }
        });
    }

    @Override
    public void onBackPressed() {
    }

    @Override
    protected void onActivityResult(int codigoSolicitud, int codigoResultado, Intent datos) {
        switch (codigoSolicitud) {
            case INTRODUCIR_PATRON: {
                switch (codigoResultado) {
                    case RESULT_OK:
                        // El gesto es correcto
                        Intent intent = new Intent(this, CameraActivity.class);
                        startActivity(intent);
                        break;
                    case LockPatternActivity.RESULT_FAILED:
                        // El gesto es incorrecto
                        Toast.makeText(this, "El gesto no es correcto.", Toast.LENGTH_LONG).show();
                        break;
                    case RESULT_CANCELED:
                        // Se cancela la tarea
                        Toast.makeText(this, "Se ha cancelado la tarea.", Toast.LENGTH_LONG).show();
                        break;
                }
                break;
            }
        }
    }
}
