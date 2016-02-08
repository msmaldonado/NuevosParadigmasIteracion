/**
 * Copyright 2014 Javier Moreno, Alberto Quesada
 *
 * This file is part of appMovimientoSonido.
 *
 * appMovimientoSonido is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * appMovimientoSonido is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with appMovimientoSonido.  If not, see <http://www.gnu.org/licenses/>.
 */

package com.PuntoMovimientoSonido.appmovimientosonido;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.os.Bundle;
import android.util.Log;
import android.widget.TextView;
import android.widget.Toast;

/**
 * Class MovementSoundActivity.
 * Controls the exectuion of the APP. Launches interface.
 *
 * @author Antonio Solis Izquierdo
 * @author Juan Antonio Velasco Gómez
 * @version 10.02.2016
 * @since 10.02.2016
 */
public class MovementSoundActivity extends Activity implements
		SensorEventListener {

	private SensorManager mSensorManager;
	private Sensor mAcceleSensor;
	private TextView result;

	private double cY;

	private boolean first = true;
	private boolean arriba = false;
	private boolean movement_ok = false;

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
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_movement_sound);
		result = (TextView) findViewById(R.id.action_result);

		// Get an instance of the sensor service
		mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
		mAcceleSensor = mSensorManager
				.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);

		PackageManager PM = this.getPackageManager();
		boolean accelerometer = PM
				.hasSystemFeature(PackageManager.FEATURE_SENSOR_ACCELEROMETER);

		if (accelerometer) {
			Toast.makeText(getApplicationContext(),
					"Accelerometer sensor is present in this device",
					Toast.LENGTH_LONG).show();
		} else {
			Toast.makeText(
					getApplicationContext(),
					"Sorry, can't do nothing...Your device doesn't have accelerometer sensor",
					Toast.LENGTH_LONG).show();
		}
	}

	/**
	 * Reproduce a sound. When it finish, activity goes off.
	 */
	private void playSound() {
		result.setText(R.string.playing_sound);
		MediaPlayer mp1 = MediaPlayer.create(MovementSoundActivity.this,
				R.raw.sonido);
		mp1.start();
		mp1.setOnCompletionListener(new OnCompletionListener() {
			public void onCompletion(MediaPlayer mp) {
				mp.release();
				Intent returnIntent = new Intent();
				setResult(RESULT_OK, returnIntent);
				finish();
			};
		});
	}

	/**
	 * Called when sensor values have changed. The length and contents of the
	 * values array vary depending on which sensor is being monitored
	 *
	 * Detects the move left and right when sound is reproduce
	 * 
	 * @param event
	 *            Object SensorEvent controls sensor values
	 * 
	 * @see playSound
	 * 
	 */
	@Override
	public final void onSensorChanged(SensorEvent event) {
		double Y = event.values[1];

		if (first) {
			cY = Y;
			first = false;
			result.setText("Mueva el dispositivo hacia abajo enérgicamente.");
		} else if (!movement_ok) {
			if (Y > cY + 20) {
				result.setText("Mueva el dispositivo hacia arriba enérgicamente.");
				arriba = true;
			} else if (arriba && (Y < cY - 20)) {
				movement_ok = true;
			}
			cY = Y;

			if (movement_ok)
				playSound();
		}
	}

	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy) {
		// TODO Auto-generated method stub

	};

	@Override
	protected void onResume() {
		super.onResume(); // registro del listener
		mSensorManager.registerListener(this, mAcceleSensor,
				SensorManager.SENSOR_DELAY_GAME);
	}

	@Override
	protected void onStop() { // anular el registro del listener
		mSensorManager.unregisterListener(this);
		super.onStop();
	}
}
