(
var stoneHarmonics = [];
var stoneHarmonicGlowHits = [];
var stoneHarmonicColoredHits = [];
var focusOnPartialHit = [];
var laserHarmonics = [];

var laserVoices = [];

var numStones;
var addr = NetAddr.new("127.0.0.1", 57120);

var currentHarmonyDegrees = 0;
var currentHarmonyMagnitude = 0;

var stage3Progression = 0;
var activeFundamental = 55;

// Just intonation intervals
~unison = 1;
~major_3rd = 5/4;
~perfect_fifth = 3/2;
~major_7th = 15/8;
~minor_7th = 16/9;
~major_2nd = 9/8;
~major_6th = 5/3;
~perfect_fourth = 4/3;
~perfect_octave = 2;

// Global defs
~numStones = 20;
~fundamental = 55;

// Chord recipes
~notes_per_chord = 3;

// lets assume we get A3 for free
~a_major_notes_1 = [~perfect_fifth/~perfect_octave, ~unison, ~perfect_fourth];
~a_major_notes_2 = [~perfect_fifth/~perfect_octave, ~unison, ~major_3rd];
~a_major_tuning = ~unison;
~a_major_freqs_1 = [-1, -1, -1];
~a_major_freqs_2 = [-1, -1, -1];

~e_major_notes_1 = [~unison, ~major_3rd, ~perfect_fifth];
~e_major_notes_2 = [~unison, ~major_3rd, ~perfect_fourth];
~e_major_tuning = ~a_major_tuning * (~perfect_fifth/~perfect_octave);
~e_major_freqs_1 = [-1, -1, -1];
~e_major_freqs_2 = [-1, -1, -1];

~g_major_notes_1 = [~perfect_fifth/~perfect_octave, ~unison, ~major_3rd];
~g_major_notes_2 = [~perfect_fifth/~perfect_octave, ~unison, ~major_2nd];
~g_major_tuning = ~a_major_tuning * (~minor_7th/~unison);
~g_major_freqs_1 = [-1, -1, -1];
~g_major_freqs_2 = [-1, -1, -1];

~d_major_notes_1 = [~unison, ~major_3rd, ~perfect_fifth];
~d_major_notes_2 = [~unison, ~major_2nd, ~perfect_fifth];
~d_major_tuning = ~g_major_tuning * (~perfect_fifth/~perfect_octave);
~d_major_freqs_1 = [-1, -1, -1];
~d_major_freqs_2 = [-1, -1, -1];

~harmony_freqs_array_1 = [];
~harmony_freqs_array_2 = [];

SynthDef(\wah,{ |freq = 65.41|
	// Start with 15 partials all at equal gain
	var numPartials = 15;
	var sig = Mix.fill(numPartials, { |i|
		var index = i + 1;
		SinOsc.ar(freq * index, 0, 1/numPartials);
	});

	// Use subtractive synthesis to shape the sound over time
	var lowPassEnv = Env.new(levels: [0, 3000], times: [3], curve:[2, 2]);
	var highPassEnv = Env.new(levels: [0, 0, 3000], times: [1.5, 3], curve:[2, 2]);
	var overallEnv = Env.new(levels: [1, 1, 0], times: [2.5, 3], curve: [2, 2]);

	// Also pan the sound from left to right as it evolves
	var panEnv = Env.new(levels: [-1, 1], times: [5]);

	sig = LPF.ar(sig, 1000);
	sig = RLPF.ar(sig, EnvGen.ar(lowPassEnv), 0.5);
	sig = HPF.ar(sig, EnvGen.ar(highPassEnv));
	sig = sig * EnvGen.ar(overallEnv, doneAction: 2);

	// Add some reverb, delay, and panning
	sig = FreeVerb.ar(sig, 0.25, 0.8, 0.7);
	sig = (0.7 * sig) + (0.3 * DelayN.ar(sig, 0.2, 0.2));
	sig = Pan2.ar(sig, EnvGen.kr(panEnv));

	Out.ar(0, 0.5 * sig);
}).add;

SynthDef(\bell, { |freq = 440, dur = 0.1, pan = 0|
	// Get ready for some local feedback delay
	var local = LocalIn.ar(2);

	// This bell sound has two subharmonics at the fifth and the octave
	var lowerRoot = freq / 2;
	var lowerFifth = lowerRoot * 3 / 2;

	// These subharmonics are not perfectly in tune. Pitch wobble is added to add character to the bell
	var lowerFifthWithPitchWobble = (lowerFifth * 1.01) + LFNoise2.ar(3, lowerFifth/50, lowerFifth/(-100));
	var lowerRootDetuned = lowerRoot + LFNoise2.ar(1, lowerRoot/25, lowerRoot/(-50));

	// FM synthesis
	var mod1 = SinOsc.ar(lowerFifthWithPitchWobble*2.02, 0, lowerFifthWithPitchWobble, 0); // I literally made this up idk what it's doing
	var car1 = SinOsc.ar(lowerRootDetuned + mod1, 0, 1, 0); // Octave subharmonic carrier
	var car2 = SinOsc.ar(lowerFifthWithPitchWobble + mod1, 0, 1, 0); // Fifth subharmonic carrier

	// I decided that we will actually hear the fundamental 2 octaves up. I don't know why, it just sounds better that way
	var upperOctave = (freq*4);
	var mod2 = SinOsc.ar(upperOctave * 1.5, 0, freq*16, 0);
	var car3 = SinOsc.ar(upperOctave + mod2, 0, 1, 0); // Fundamental / upper harmonics carrier

	// Add some high end noise too
	var noise = HPF.ar(PinkNoise.ar(1, 0), 4000);

	// Different envelopes shape each FM carrier and noise generator
	var env1 = Env.new(levels:[0, 1, 0], times: [0.25, 2-0.25], curve: [-5, -10]);
	var env2 = Env.new(levels:[0, 1, 0], times: [0.1, 2-0.1], curve: [-5, -10]);
	var env3 = Env.new(levels:[0, 1, 0], times: [0.01, 2-0.01], curve: [-5, -10]);
	var noiseEnv = Env.new(levels:[0, 1, 0], times: [0.01, 2-0.01], curve: [-5, -10]);

	// How much of each carrier to we hear?
	var mixCar1 = 0.007; // Root
	var mixCar2 = 0.007; // Fifth
	var mixCar3 = 0.03; // Fundamental / upper harmonics
	var noiseMix = 0.03;

	var mixedSig, sig, upperDelay;

	// Use local feedback for a delay effect on the sixteenth note
	local = DelayN.ar(local, ~sixteenth_tick + 0.01, ~sixteenth_tick + 0.01);
	local = (0.5 * car3*EnvGen.ar(env3)) + (0.5 * local);

	mixedSig = 0.5*(mixCar1*car1*EnvGen.ar(env1)) + (mixCar2*car2*EnvGen.ar(env2)) + (mixCar3*local) + (noiseMix*noise*EnvGen.ar(noiseEnv, doneAction: 2));

	LocalOut.ar(local);
	Out.ar(0, Pan2.ar(FreeVerb.ar(mixedSig, 0.5, 0.8, 0.7), pan));
}).add;

SynthDef.new(\laserModeVoice, {
	arg fund, mul, finalFade = 1, pan = 0,
	amps = #[0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
	additionalAmp_1 = #[0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
	additionalAmp_2 = #[0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
	additionalAmp_3 = #[0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

	var sig;

	sig = Mix.fill(amps.size, { |i|
		var index, freq, mod;
		index = i + 1;
		freq = fund * index;

		//mod = SinOsc.ar((1 + modAdd) * freq, 0, 1 * freq, 0);
		SinOsc.ar(freq, 0, (1/amps.size) * amps[i] * additionalAmp_1[i] * additionalAmp_2[i] * additionalAmp_3[i] * finalFade);
	});

	Out.ar(0, sig);
}).add;

SynthDef.new(\laserModeSine, {
	arg freq, mul, atten = 1, pan = 0;
	var sig;
	sig = SinOsc.ar(freq, 0, mul * atten, 0);

	Out.ar(0, Pan2.ar(sig, pan));
}).add;

SynthDef.new(\stoneHarmonic, {
	arg freq, mul, t_trig = 0, atten = 1, partialNum, fund, pan = 0;

	var freq_q = fund * (partialNum + 1);

	var sig, env;
	env = EnvGen.kr(Env.perc(0.01, 1.0, 1.0), t_trig, doneAction:0);
	sig = SinOsc.ar(freq_q, 0, mul * atten, 0) * env;

	Out.ar(0, Pan2.ar(sig, pan));
}).add;


SynthDef.new(\focusOnPartialHit, {
	arg freq, mul, t_trig = 0, atten = 1, partialNum, fund, pan = 0;

	var freq_q = fund * (partialNum + 1);
	var sig, sig1, sig2, car1, car2, car3, car4, car5, car6, mod1, mod2, env1, env2, env3, lfo1;

	env1 = EnvGen.kr(Env.perc(0.01, 1.0, 1.0), t_trig, doneAction: 0);
	env2 = EnvGen.kr(Env.new([0, 1, 0], [1, 0.75], [2, -2]), t_trig, doneAction: 0);
	env3 = EnvGen.kr(Env.new([0, 1, 0.8], [1, 0.5], [4, -4]), t_trig, doneAction: 0);

	lfo1 = SinOsc.kr(env3 * 2, 0, 2 * env3, -1);

	mod1 = SinOsc.ar(freq_q, 0, freq_q * 4);

	car1 = SinOsc.ar(freq_q, 0, 1, 0);
	car2 = SinOsc.ar((freq_q * 3 / 2) + (mod1 * 0.25) + (lfo1 * freq_q/100), 0, 1, 0);
	car3 = SinOsc.ar((freq_q * 2) + (mod1 * 0.5) + (lfo1 * freq_q/100), 0, 1, 0);
	car4 = SinOsc.ar((freq_q * 2 * 5 / 4) + (mod1 * 1) + (lfo1 * freq_q/100), 0, 1, 0);
	car5 = SinOsc.ar((freq_q * 4) + (mod1 * env3), 0, 1, 0);
	car6 = SinOsc.ar(freq_q * 4 * 5 / 4, 0, 1, 0);

	sig1 = ((car1 * 0.35) + (car2 * 0.2) + (car3 * 0.15) + (car4 * 0.05)) * env1;
	sig2 = ((car5 * 0.2) + (car6 * 0.05)) * env2;
	sig = sig1 + sig2;
	sig = sig * 0.4;

	Out.ar(0, Pan2.ar(sig, pan));
}).add;

SynthDef.new(\stoneHarmonicGlowHit, {
	arg freq, mul, t_trig = 0, atten = 1, partialNum, fund, pan = 0;

	var freq_q = fund * (partialNum + 1);

	var volumeEnv = Env.new([0, 1, 0.8, 0], [0.01, 0.2, 1.0]);
	var modEnv = Env.new([0, 1, 0.25], [0.6, 0.6], [2, 2]);
	var modEnvVal = EnvGen.kr(modEnv, t_trig, doneAction:0);

	var mod2 = SinOsc.ar(freq_q*3.9, 0, 4*freq_q*modEnvVal);
	var mod1 = SinOsc.ar(freq_q*1.9, 0, 4*freq_q*modEnvVal);
	var car1 = SinOsc.ar(freq_q + mod1, 0, 0.7);
	var car2 = SinOsc.ar(freq_q + mod2, 0, 0.3);

	var sum = car1 + car2;
	var sig = sum * EnvGen.kr(volumeEnv, t_trig, doneAction:0) * mul * 0.5;

	Out.ar(0, Pan2.ar(sig, pan));
}).add;

SynthDef.new(\stoneHarmonicColoredHit, {
	arg freq, mul, t_trig = 0, atten = 1, mode, partialNum, fund, pan = 0;

	var freq_q = fund * (partialNum + 1);

	var env = EnvGen.kr(Env.perc(0.01, 1.0, 1.0), t_trig, doneAction:0);
	var mod1 = SinOsc.ar(freq_q * (mode + 1), 0, freq_q * 1);
	var car1 = SinOsc.ar(freq_q + mod1, 0, 1);

	var sig = car1 * env * mul * atten * 0.5;

	Out.ar(0, Pan2.ar(sig, pan));
}).add;

// msg[1] = number of synths to init
OSCdef(\initSynths, {|msg|
	var a3, laserVoiceMul, ampZeros;
	"initSynths".postln;
	stoneHarmonics = Array.new();
	stoneHarmonicGlowHits = Array.new();
	stoneHarmonicColoredHits = Array.new();
	focusOnPartialHit = Array.new();

	~numStones = msg[1];
	~numStones.do({ |i|
		stoneHarmonics = stoneHarmonics.add(Synth.new(\stoneHarmonic, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \partialNum, i, \fund, ~fundamental, \t_trig, 0, \pan, cos((i/~numStones)*2*pi)]));
		stoneHarmonicGlowHits = stoneHarmonicGlowHits.add(Synth.new(\stoneHarmonicGlowHit, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \partialNum, i, \fund, ~fundamental, \t_trig, 0, \pan, cos((i/~numStones)*2*pi)]));
		stoneHarmonicColoredHits = stoneHarmonicColoredHits.add(Synth.new(\stoneHarmonicColoredHit, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \partialNum, i, \fund, ~fundamental, \t_trig, 0, \pan, cos((i/~numStones)*2*pi)]));
		focusOnPartialHit = focusOnPartialHit.add(Synth.new(\focusOnPartialHit, [\freq, 55 * (i + 1), \mul, (1.0/msg[1].squared), \partialNum, i, \fund, ~fundamental, \t_trig, 0, \pan, cos((i/~numStones)*2*pi)]));
		laserHarmonics = laserHarmonics.add(Synth.new(\laserModeSine, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \atten, 0, \pan, cos((i/~numStones)*2*pi)]));
	});

	laserVoices = Array.new();
	laserVoiceMul = 1.0/~notes_per_chord;
	ampZeros = Array.new();
	~numStones.do({|i|
		ampZeros.add(0);
	});

	~notes_per_chord.do({|i|
		laserVoices = laserVoices.add(Synth.new(\laserModeVoice, [\fund, ~fundamental, \mul, 0.33, \amps, ampZeros]));
	});

	a3 = ~fundamental * 3;
	// Calculate chord freqs
	~a_major_notes_1.size.do({ |i|
		~a_major_freqs_1[i] = a3 * ~a_major_tuning * ~a_major_notes_1[i];
		~a_major_freqs_2[i] = a3 * ~a_major_tuning * ~a_major_notes_2[i];
	});

	~e_major_notes_1.size.do({ |i|
		~e_major_freqs_1[i] = a3 * ~e_major_tuning * ~e_major_notes_1[i];
		~e_major_freqs_2[i] = a3 * ~e_major_tuning * ~e_major_notes_2[i];
	});

	~g_major_notes_1.size.do({ |i|
		~g_major_freqs_1[i] = a3 * ~g_major_tuning * ~g_major_notes_1[i];
		~g_major_freqs_2[i] = a3 * ~g_major_tuning * ~g_major_notes_2[i];
	});

	~d_major_notes_1.size.do({ |i|
		~d_major_freqs_1[i] = a3 * ~d_major_tuning * ~d_major_notes_1[i];
		~d_major_freqs_2[i] = a3 * ~d_major_tuning * ~d_major_notes_2[i];
	});

	~harmony_freqs_array_1 = [~a_major_freqs_1, ~e_major_freqs_1, ~g_major_freqs_1, ~d_major_freqs_1];
	~harmony_freqs_array_2 = [~a_major_freqs_2, ~e_major_freqs_2, ~g_major_freqs_2, ~d_major_freqs_2];

}, '/initSynths');

// no params
OSCdef(\freeAllSynths, {|msg|
	"freeAllSynths".postln;
	stoneHarmonics.size.do({ |i|
		stoneHarmonics[i].free;
		i.postln;
	});
}, '/freeAllSynths');

// no params
OSCdef(\playAll, {|msg|
	"playAll".postln;
	stoneHarmonics.size.do({ |i|
		stoneHarmonics[i].set(\atten, msg[2]);
		stoneHarmonics[i].set(\t_trig, 1);
		i.postln;
	});
}, '/playAll');


// msg[1] = target stone (target harmonic)
OSCdef(\playStoneNote, {|msg|
	"playStoneNote".postln;
	stoneHarmonics[msg[1]].set(\atten, msg[2]);
	stoneHarmonics[msg[1]].set(\t_trig, 1);
}, '/playStoneNote');

OSCdef(\playInitialStoneSelectStage1, {|msg|
	"playInitialStoneSelectStage1".postln;
	Synth(\bell, [\freq, activeFundamental * (msg[1] + 1) * 2]);
}, '/playInitialStoneSelectStage1');

// msg[1] = target stone (target harmonic)
OSCdef(\playStoneNoteGlowHit, {|msg|
	"playStoneNoteGlowHit".postln;
	stoneHarmonicGlowHits[msg[1]].set(\atten, msg[2]);
	stoneHarmonicGlowHits[msg[1]].set(\t_trig, 1);
}, '/playStoneNoteGlowHit');

OSCdef(\playStoneNoteColoredHit, {|msg|
	"playStoneNoteColoredHit".postln;
	stoneHarmonicColoredHits[msg[1]].set(\atten, msg[2]);
	stoneHarmonicColoredHits[msg[1]].set(\t_trig, 1);
	stoneHarmonicColoredHits[msg[1]].set(\mode, msg[3]);
}, '/playStoneNoteColoredHit');

OSCdef(\updateHarmonyCircleData, {|msg|
	"updateHarmonyCircleData".postln;
	("angle = " + msg[1]).postln;
	("magnitude = " + msg[2]).postln;

	currentHarmonyDegrees = msg[1] % 360;
	currentHarmonyMagnitude = msg[2];

}, '/updateHarmonyCircleData');

OSCdef(\playFocusOnPartialHit, {|msg|
	"playFocusOnPartialHit".postln;
	focusOnPartialHit[msg[1]].set(\t_trig, 1);
}, '/playFocusOnPartialHit');

OSCdef(\keyChange, {|msg|
	"keyChange".postln;
	activeFundamental = ~fundamental * 2.pow(msg[1]/12);
	~numStones.do({|i|
		stoneHarmonics[i].set(\fund, activeFundamental);
		stoneHarmonicGlowHits[i].set(\fund, activeFundamental);
		stoneHarmonicColoredHits[i].set(\fund, activeFundamental);
	});
}, '/keyChange');

OSCdef(\keyComplete, {
	Synth(\wah, [\freq, activeFundamental * 3]);
}, '/keyComplete');

// I don't think we need this any more
OSCdef(\activateLaserMode, { |msg|
	"activateLaserMode".postln;
	numStones.do({|i|
		laserHarmonics[i].set(\atten, 1);
	});
}, '/activateLaserMode');

OSCdef(\updateStage3FinalFade, {|msg|
	~notes_per_chord.do({|i|
		laserVoices[i].set(\finalFade, msg[1]);
	});
}, '/updateStage3FinalFade');

OSCdef(\updateStage3Progression, { |msg|
	var additionalAmp_1, additionalAmp_2, additionalAmp_3;
	"updateStage3Progression".postln;
	stage3Progression = msg[1];
	stage3Progression.postln;

	~notes_per_chord.do({|i|
		laserVoices[i].set(\finalFade, 1);
	});

	additionalAmp_1 = Array.new();
	~numStones.do({|i|
		if((i % 2) == 0,
			{additionalAmp_1 = additionalAmp_1.add(1)},
			{additionalAmp_1 = additionalAmp_1.add(((1 - stage3Progression) * 0.8) + 0.2)}
		);
	});

	additionalAmp_2 = Array.new();
	~numStones.do({|i|
		if((stage3Progression < 0.5), {
			// No additional attenuation
			additionalAmp_2 = additionalAmp_2.add(1)
		},{
			if(i == 2, {
				additionalAmp_2 = additionalAmp_2.add(1);
			}, {
				if((stage3Progression > 0.9), {
					// Maximum attenuation from this stage
					additionalAmp_2 = additionalAmp_2.add(0.1);
				}, {
					var t = (stage3Progression - 0.5) / 0.4;
					additionalAmp_2 = additionalAmp_2.add(((1 - t) * 0.9) + 0.1);
				});
			});
		});
	});

	additionalAmp_3 = Array.new();
	~numStones.do({|i|
		if((stage3Progression < 0.9), {
			// No additional attenuation
			additionalAmp_3 = additionalAmp_3.add(1);
		},{
			if(i == 2, {
				additionalAmp_3 = additionalAmp_3.add(1);
			}, {
				var t = (stage3Progression - 0.9) / 0.1;
				var atten = 1 - t;
				additionalAmp_3 = additionalAmp_3.add(atten);
			});
		});
	});

	~notes_per_chord.do({|i|
		laserVoices[i].set(\additionalAmp_1, additionalAmp_1, \additionalAmp_2, additionalAmp_2, \additionalAmp_3, additionalAmp_3);
	});
}, '/updateStage3Progression');

OSCdef(\updateLaserData, {|msg|
	/*msg[1].do({
		|i|
		laserHarmonics[i].set(\atten, msg[2 + i]);
	});*/

	var amps = Array.new();
	var harmony_A_freqs_1, harmony_A_freqs_2, harmony_A_freqs, harmony_B_freqs_1, harmony_B_freqs_2, harmony_B_freqs, harmony_interp_freqs, angleDiv, angleMod, lerped;

	harmony_interp_freqs = Array.new();
	msg[1].do({|i|
		amps = amps.add(msg[2 + i]);
	});

	// Update amps
	~notes_per_chord.do({|i|
		laserVoices[i].set(\amps, amps);
	});

	// Update harmonies
	angleDiv = (currentHarmonyDegrees / 90).floor;
	angleMod = (currentHarmonyDegrees % 90) / 90;

	harmony_A_freqs_1 = ~harmony_freqs_array_1[angleDiv];
	harmony_A_freqs_2 = ~harmony_freqs_array_2[angleDiv];
	harmony_B_freqs_1 = ~harmony_freqs_array_1[(angleDiv + 1) % 4];
	harmony_B_freqs_2 = ~harmony_freqs_array_2[(angleDiv + 1) % 4];
	harmony_A_freqs = Array.new();
	harmony_B_freqs = Array.new();

	~notes_per_chord.do({|i|
		harmony_A_freqs = harmony_A_freqs.add((harmony_A_freqs_1[i] * (currentHarmonyMagnitude)) + (harmony_A_freqs_2[i] * (1.0 - currentHarmonyMagnitude)));
		harmony_B_freqs = harmony_B_freqs.add((harmony_B_freqs_1[i] * (currentHarmonyMagnitude)) + (harmony_B_freqs_2[i] * (1.0 - currentHarmonyMagnitude)));
	});

	~notes_per_chord.do({|i|
		lerped = (harmony_A_freqs[i] * (1.0 - angleMod)) + (harmony_B_freqs[i] * (angleMod));
		harmony_interp_freqs = harmony_interp_freqs.add(lerped);
	});

	~notes_per_chord.do({|i|
		laserVoices[i].set(\fund, harmony_interp_freqs[i]);
	});

}, '/updateLaserData');

addr.sendMsg("/initSynths", 20);
)

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// KEY CHANGE
(
b = NetAddr.new("127.0.0.1", 57120);
b.sendMsg("/keyChange", 2);
)

// FREE ALL
(
b = NetAddr.new("127.0.0.1", 57120);
b.sendMsg("/freeAllSynths");
)

(
b = NetAddr.new("127.0.0.1", 57120);
b.sendMsg("/playInitialStoneSelectStage1", 4);
)

// PLAY NOTE
(
b = NetAddr.new("127.0.0.1", 57120);
b.sendMsg("/playStoneNote", 0, 1);
b.sendMsg("/playStoneNote", 1, 1);
b.sendMsg("/playStoneNote", 2, 1);
b.sendMsg("/playStoneNote", 3, 1);
b.sendMsg("/playStoneNote", 4, 1);
b.sendMsg("/playStoneNote", 5, 1);
b.sendMsg("/playStoneNote", 6, 1);
b.sendMsg("/playStoneNote", 7, 1);
b.sendMsg("/playStoneNote", 8, 1);
b.sendMsg("/playStoneNote", 9, 1);
)

// PLAY ALL
(
b = NetAddr.new("127.0.0.1", 57120);
b.sendMsg("/playAll", 20, 1);
)
