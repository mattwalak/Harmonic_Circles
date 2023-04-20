/*
"ifconfig | grep 'inet '".unixCmd;

n = NetAddr("127.0.0.1", 57120); // You would typically supply your phone or alternate language here, but it's not necessary unless you want responders receiving from different servers.

NetAddr.langPort;  // **should** be 57120. *or 21*

// Turns the trace on and dumps all incoming OSC messages.
OSCFunc.trace(bool: true, hideStatusMsg: true);  // I don't need no stinkin' status messages!
*/

// thisProcess.platform.recordingsDir;
// s.record();

(
var fund = 50;
var bassOptions = [0, 5];
var melodyOptions = [0, 4, 5, 9];
var harmonyOptions = [0, 2, 4, 5, 7, 9, 11];

/////////////////////////////////////////////////////
/////////////// HELPER FUNCTIONS ////////////////////
/////////////////////////////////////////////////////

var scaleToGain = {
	arg scale;
	scale.dbamp
};

/////////////////////////////////////////////////////
/////////////// SOUND FUNCTIONS /////////////////////
/////////////////////////////////////////////////////

OSCdef(\highNote, {|msg|
	x = {
		var sig, env, fundamental, gain;
		env = XLine.kr(1, 0.01, 1, doneAction: 2);
		fundamental = (harmonyOptions.choose + 60).midicps;
		fundamental = fundamental * rrand(1, 2);
		gain = scaleToGain.value(msg[1]);
		sig = (SinOsc.ar(fundamental * 1.1) * 0.1 * env
		    + SinOsc.ar((fundamental * 2.2)) * 0.1 * env
		    + SinOsc.ar((fundamental * 3.3)) * 0.1 * env
			+ SinOsc.ar((fundamental * 4.4)) * 0.1 * env) * gain * 2 / 4.0;
	}.play;
	"play high note".postln;
}, '/highNote');

OSCdef(\melodyNote, {|msg|
	x = {
		var sig, env, fundamental, gain;
		env = XLine.kr(1, 0.01, 1, doneAction: 2);
		fundamental = (melodyOptions.choose + 48).midicps;
		gain = scaleToGain.value(msg[1]);
		sig = Pulse.ar(fundamental) * 0.1 * env * gain * 2;
	}.play;
	"play melody note".postln;
}, '/melodyNote');

OSCdef(\bassNote, {|msg|
	x = {
		var sig, env, fundamental, gain;
		env = XLine.kr(1, 0.01, 1, doneAction: 2);
		fundamental = (bassOptions.choose + 24).midicps;
		gain = scaleToGain.value(msg[1]);
		sig = Saw.ar(fundamental * rrand(1, 2)) * 0.1 * env * gain * 2;
	}.play;
	"play bass note".postln;
}, '/bassNote');

/////////////////////////////////////////////////////
/////////////// SET OPTIONS /////////////////////////
/////////////////////////////////////////////////////

OSCdef(\setBassOptions, {|msg|
	bassOptions = [];
	msg[1].do({ |i|
		bassOptions = bassOptions.add(msg[2 + i]);
	});

	"bass options".postln;
	bassOptions.postln;

}, '/setBassOptions');

OSCdef(\setMelodyOptions, {|msg|
	melodyOptions = [];
	msg[1].do({ |i|
		melodyOptions = melodyOptions.add(msg[2 + i]);
	});

	"melody options:".postln;
	melodyOptions.postln;

}, '/setMelodyOptions');

OSCdef(\setHarmonyOptions, {|msg|
	harmonyOptions = [];
	msg[1].do({ |i|
		harmonyOptions = harmonyOptions.add(msg[2 + i]);
	});

	"harmony options:".postln;
	harmonyOptions.postln;

}, '/setHarmonyOptions');

)
