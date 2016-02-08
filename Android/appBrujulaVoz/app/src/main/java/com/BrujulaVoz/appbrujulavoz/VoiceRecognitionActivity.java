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

import java.util.ArrayList;
import java.util.List;

import com.NPI.appbrujulavoz.R;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

/**
 * Class VoiceRecognitionActivity. Controls the patterns to detect.
 */
public class VoiceRecognitionActivity extends Activity implements OnClickListener {

	private static final int REQUEST_CODE = 1234;
	private TextView card;//Texto que muestra punto cardinal con tolerancia elegiada
	private TextView naveg;// Se nos informa de que ya podemos comenzar la navegación
	private static final String TAG = "VoiceRecognitionActivity";
    private String cardinal;//Almacena el valor del cardinal
	private float tolerancia;
	private Button speakButton; //botón para mandar por voz el punto cardinal
	private Button navigationButton;//botón para iniciar la navegación
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
		setContentView(R.layout.activity_voice);
		speakButton = (Button) findViewById(R.id.button_speak);
		navigationButton = (Button) findViewById(R.id.button_navigation);
		card = (TextView) findViewById(R.id.card);

		PackageManager pm = getPackageManager();
		List<ResolveInfo> activities = pm.queryIntentActivities(new Intent(
				RecognizerIntent.ACTION_RECOGNIZE_SPEECH), 0);
		if (activities.size() == 0) {
			speakButton.setEnabled(false);
			speakButton.setText("Recognizer not present");
		}
		speakButton.setOnClickListener(this);
		navigationButton.setOnClickListener(this);

	}

	/**
	 * Called when a view has been clicked.
	 * 
	 * @param view
	 *            The view that was clicked.
	 */
	public void onClick(View v) {
		if (v.getId() == R.id.button_speak)
			startRecognition();
		else if (v.getId() == R.id.button_navigation) {
			Intent returnIntent = new Intent();
            returnIntent.putExtra("cardinal", cardinal);
			returnIntent.putExtra("tolerancia", tolerancia);
			setResult(RESULT_OK, returnIntent);
			finish();
		}
	}

	/**
	 * Iniciates voice recognite when calling the activity
	 * voice.recognition.test.
	 */
	public void startRecognition() {
		Intent voice_intent = new Intent( RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
		voice_intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
		voice_intent.putExtra(RecognizerIntent.EXTRA_CALLING_PACKAGE, "voice.recognition.test");
		voice_intent.putExtra(RecognizerIntent.EXTRA_MAX_RESULTS, 5);
		startActivityForResult(voice_intent, REQUEST_CODE);
	}

	/**
	 * Handle the results from the voice recognition activity.
	 */
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == REQUEST_CODE && resultCode == RESULT_OK) {
			ArrayList<String> matches = data
					.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
			checkResults(matches);
		}
		super.onActivityResult(requestCode, resultCode, data);
	}

	/**
	 * Compare both detected results to see if they are correct.
	 * 
	 * @param matches
	 *            List of char detected in the voice detection.
	 * 
	 * @see checkData
	 */
	private void checkResults(ArrayList<String> matches) {
		checkData(matches, card);
		speakButton.setText(getResources().getString(R.string.button_speak_again));
		if (tolerancia != 0F) {
			navigationButton.setEnabled(true);//activamos el botón de navegación
			naveg = (TextView) findViewById(R.id.naveg);//ponemos el texto de que ya podemos realizar la navegación
			naveg.setText("Puede comenzar navegación o indicar otro punto cardinal y su tolerancia.");
		}
		else
			navigationButton.setEnabled(false);
	}

	/**
	 * Compare both detected results to see if they are correct.
	 * 
	 * @param data
	 *            List of char detected in the voice detection.
	 * @param word
	 *            Word to be found on the cad of char.
	 * @param text
	 *            TextView where result will be define if its correct.
	 * 
	 * @see checkResults
	 */
	private void checkData(ArrayList<String> data, TextView text) {
		/*Buscamos el punto cardinal y a continuación su toleracia. Almacenamos el cardinal y la tolerancia*/
		float aux = search(data, "norte");
		if (aux != 0F) {
			tolerancia = aux;
            cardinal = "norte";
			text.setText("Norte" + " : " + aux);
		}
         aux = search(data, "sur");
        if (aux != 0F) {
            tolerancia = aux;
            cardinal = "sur";
            text.setText("Sur" + " : " + aux);
        }
         aux = search(data, "este");
        if (aux != 0F) {
            tolerancia = aux;
            cardinal = "este";
            text.setText("Este" + " : " + aux);
        }
         aux = search(data, "oeste");
        if (aux != 0F) {
            tolerancia = aux;
            cardinal = "oeste";
            text.setText("Oeste" + " : " + aux);
        }
	}

	/**
	 * Looks for the word in the list of cad of char and gives back the
	 * numeric value for that word
	 * @param data
	 *            List of char detected in the voice detection.
	 * 
	 * @param word
	 *            Word to search.
	 * 
	 * @see checkData
	 * @see checkResults
	 */
	private float search(final ArrayList<String> data, String word) {
		float f = 0F;
		boolean found = false;
		for (int i = 0; i < data.size(); i++) {
			Log.d(TAG, "result " + data.get(i));
			if (!found && data.get(i).contains(word)) {
				String s = data.get(i);
				s = s.replaceAll("menos", "-");
				s = s.replaceAll("con", ".");
				String result = "";
				for (int j = s.indexOf(word) + word.length(); j < s.length(); j++) {
					char c;
					if (Character.isDigit(s.charAt(j)) || s.charAt(j) == ','
							|| s.charAt(j) == '.' || s.charAt(j) == '-') {
						if (s.charAt(j) == ',')
							c = '.';
						else
							c = s.charAt(j);
						result += c;
					} else if (s.charAt(j) != ' ') // Letra
						break;
				}
				try {
					f = Float.parseFloat(result);
					found = true;
				} catch (NumberFormatException e) {
					Log.e(TAG, result);
				}
			}
		}
		return f;
	}
}
