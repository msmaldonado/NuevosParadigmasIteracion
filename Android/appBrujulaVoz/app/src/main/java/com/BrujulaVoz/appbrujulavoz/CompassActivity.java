/**
 * Copyright 2016
 * @author Cristina Zuheros Montes
 * @author Miguel Sánchez Maldonado
 * @version 10.02.2016
 * This file is part of appGPSVoz.
 *
 * appBrujulaVoz is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * appBrujulaVoz is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with appBrujulaVoz.  If not, see <http://www.gnu.org/licenses/>.
 */

package com.BrujulaVoz.appbrujulavoz;

import com.NPI.appbrujulavoz.R;

import android.hardware.GeomagneticField;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.provider.Settings;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.view.animation.Animation;
import android.view.animation.RotateAnimation;
import android.widget.ImageView;
import android.widget.TextView;

/**
 * Class CompassActivity. Controls the pattern to detect.
 *
 */
public class CompassActivity extends Activity implements SensorEventListener {

    // Views donde se cargaran los elementos del XML
    private TextView txtAngle;
    private ImageView imgCompass;

    // Guarda el angulo (grado) actual del compass
    private float currentDegree = 0f;

    // El sensor manager del dispositivo
    private SensorManager mSensorManager;

    // Guarda el punto cardinal que hemos pedido por voz
    private String cardinal;

    // El tolerancia de error permitida
    float tolerancia ;

	/**
	 * Called when the activity is starting. This is where most initialization
	 * should go: calling setContentView(int) to inflate the activity's UI,
	 * using findViewById(int) to programmatically interact with widgets in the
	 * UI, calling managedQuery(android.net.Uri, String[], String, String[],
	 * String) to retrieve cursors for data being displayed, etc.
	 * <p>
	 * You can call finish() from within this function, in which case
	 * onDestroy() will be immediately called without any of the rest of the
	 * activity lifecycle (onStart(), onResume(), onPause(), etc) executing.
	 *
	 *
	 * @param savedInstanceState
	 *            If the activity is being re-initialized after previously being
	 *            shut down then this Bundle contains the data it most recently
	 *            supplied in onSaveInstanceState(Bundle).
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_gps);

        // Se guardan en variables los elementos del layout
        imgCompass = (ImageView) findViewById(R.id.imgViewCompass);
        txtAngle = (TextView) findViewById(R.id.txtAngle);

        Bundle extras = getIntent().getExtras();
        if (extras != null) {
            cardinal = extras.getString("cardinal");
            tolerancia = extras.getFloat("error", 0F);
        }

        // Se inicializa los sensores del dispositivo android
        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
    }

	/**
	 * Method called when the app resumes his activity
	 */
	@SuppressWarnings("deprecation")
    @Override
    protected void onResume() {
        super.onResume();

        // for the system's orientation sensor registered listeners            magnetometer
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION),
                SensorManager.SENSOR_DELAY_GAME);
    }

	/**
	 * Method called when the app pauses his activity
	 */
    @Override
    protected void onPause() {
        super.onPause();

        // Se detiene el listener para no malgastar la bateria
        mSensorManager.unregisterListener(this);
    }

	/**
	 * Method called when the sensor status changes
	 */
    @Override
    public void onSensorChanged(SensorEvent event) {

        // get the angle around the z-axis rotated
        float degree = Math.round(event.values[0]);

        txtAngle.setText("Está a : " + Float.toString(degree) + " grados.\n Apunte hacia el " + cardinal);

        // create a rotation animation (reverse turn degree degrees)
        RotateAnimation ra = new RotateAnimation(
                currentDegree,
                -degree,
                Animation.RELATIVE_TO_SELF, 0.5f,
                Animation.RELATIVE_TO_SELF,
                0.5f);

        // how long the animation will take place
        ra.setDuration(50);

        // set the animation after the end of the reservation status
        ra.setFillAfter(true);

        // Start the animation
        imgCompass.startAnimation(ra);
        currentDegree = -degree;

        if(( cardinal.equals("norte") && (degree < tolerancia || degree > 360.0 - tolerancia )) ||
            ( cardinal.equals("sur") && degree > 180.0 - tolerancia && degree < 180.0 + tolerancia ) ||
            ( cardinal.equals("oeste") && degree > 90.0 - tolerancia && degree < 90.0 + tolerancia ) ||
            ( cardinal.equals("este") && degree > 270.0 - tolerancia && degree < 270.0 + tolerancia )) {
                Intent returnIntent = new Intent();
                setResult(RESULT_OK, returnIntent);
                finish();
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }
}