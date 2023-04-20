(
var stoneHarmonics = [];
var stoneHarmonicGlowHits = [];
var stoneHarmonicColoredHits = [];
var numStones;
var addr = NetAddr.new("127.0.0.1", 57120);

~numStones = 20;
~fundamental = 55;

SynthDef.new(\laserModeSine, {
	arg freq, mul, frac;
	var sig;
	sig = SinOsc.ar(freq, 0, mul * frac, 0);

	Out.ar(0, sig);
}).add;

SynthDef.new(\stoneHarmonic, {
	arg freq, mul, t_trig = 0, atten = 1, partialNum, fund;

	var freq_q = fund * (partialNum + 1);

	var sig, env;
	env = EnvGen.kr(Env.perc(0.01, 1.0, 1.0), t_trig, doneAction:0);
	sig = SinOsc.ar(freq_q, 0, mul * atten, 0) * env;

	Out.ar(0, sig);
}).add;

SynthDef.new(\stoneHarmonicGlowHit, {
	arg freq, mul, t_trig = 0, atten = 1, partialNum, fund;

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

	Out.ar(0, sig);
}).add;

SynthDef.new(\stoneHarmonicColoredHit, {
	arg freq, mul, t_trig = 0, atten = 1, mode, partialNum, fund;

	var freq_q = fund * (partialNum + 1);

	var env = EnvGen.kr(Env.perc(0.01, 1.0, 1.0), t_trig, doneAction:0);
	var mod1 = SinOsc.ar(freq_q * (mode + 1), 0, freq_q * 1);
	var car1 = SinOsc.ar(freq_q + mod1, 0, 1);

	var sig = car1 * env * mul * atten * 0.5;

	Out.ar(0, sig);
}).add;

// msg[1] = number of synths to init
OSCdef(\initSynths, {|msg|
	"initSynths".postln;
	stoneHarmonics = Array.new();
	stoneHarmonicGlowHits = Array.new();
	stoneHarmonicColoredHits = Array.new();

	msg[1].do({ |i|
		stoneHarmonics = stoneHarmonics.add(Synth.new(\stoneHarmonic, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \partialNum, i, \fund, ~fundamental, \t_trig, 0]));
		stoneHarmonicGlowHits = stoneHarmonicGlowHits.add(Synth.new(\stoneHarmonicGlowHit, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \partialNum, i, \fund, ~fundamental, \t_trig, 0]));
		stoneHarmonicColoredHits = stoneHarmonicColoredHits.add(Synth.new(\stoneHarmonicColoredHit, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \partialNum, i, \fund, ~fundamental, \t_trig, 0]));
		// stoneHarmonics = stoneHarmonics.add(Synth.new(\laserModeSine, [\freq, 55 * (i + 1), \mul, (1.0/msg[1]), \frac, 1]));
	});

	~numStones = msg[1];

	numStones = msg[1];
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

OSCdef(\keyChange, {|msg|
	var newFundamental;
	"keyChange".postln;
	newFundamental = ~fundamental * 2.pow(msg[1]/12);
	~numStones.do({|i|
		stoneHarmonics[i].set(\fund, newFundamental);
		stoneHarmonicGlowHits[i].set(\fund, newFundamental);
		stoneHarmonicColoredHits[i].set(\fund, newFundamental);
	});
}, '/keyChange');

OSCdef(\updateLaserData, {|msg|
	"update laser data".postln;
	msg[1].do({
		|i|
		stoneHarmonics[i].set(\frac, msg[2 + i]);
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
