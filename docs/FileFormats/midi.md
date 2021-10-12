# MIDI

A MIDI file is a file that stores a MIDI stream. It is a binary format and uses the `.mid` and `.midi` file extensions (`.midi` is not used for charts or supported by CH, however).

Modern MIDI charts originate from Rock Band's MIDI chart format, with some additional functionality added in by Phase Shift and Clone Hero. Older MIDI charts are slightly different and are based on GH1/2's format, but follow similar patterns.

## Table of Contents

- [Basic Infrastructure](#basic-infrastructure)
  - [Chunks](#chunks)
  - [Events](#events)
    - [Event Types](#event-types)
    - [Warning For SysEx](#warning-for-sysex)
- [Using via ChartTools](#using-via-charttools)
- [Track Names](#track-names)
- [Track Details](#track-details)
  - [5-Fret Tracks](#5-fret-tracks)
    - [5-Fret Notes](#5-fret-notes)
    - [5-Fret SysEx Events](#5-fret-sysex-events)
    - [5-Fret Text Events](#5-fret-text-events)
  - [6-Fret Tracks](#6-fret-tracks)
    - [6-Fret Notes](#6-fret-notes)
    - [6-Fret SysEx Events](#6-fret-sysex-events)
  - [Drums Tracks](#drums-tracks)
    - [Drums Notes](#drums-notes)
    - [Drums Real SysEx Events](#drums-real-sysex-events)
    - [Drums Text Events](#drums-text-events)
    - [Drums Additional Notes](#drums-additional-notes)
  - [Vocals Tracks](#vocals-tracks)
    - [Vocals Notes](#vocals-notes)
    - [Vocals Lyrics](#vocals-lyrics)
    - [Vocals Text Events](#vocals-text-events)
  - [5-Lane Keys Track (As Rock Band 3 Expects)](#5-lane-keys-track-as-rock-band-3-expects)
    - [5-Lane Keys Notes](#5-lane-keys-notes)
    - [5-Lane Keys Text Events](#5-lane-keys-text-events)
  - [Pro Keys Tracks](#pro-keys-tracks)
    - [Pro Keys Notes](#pro-keys-notes)
    - [Pro Keys Animation Notes](#pro-keys-animation-notes)
  - [Keys Real Tracks](#keys-real-tracks)
    - [Keys Real Notes](#keys-real-notes)
  - [Pro Guitar/Bass Tracks](#pro-guitarbass-tracks)
    - [Pro Guitar/Bass 17-Fret Notes](#pro-guitarbass-17-fret-notes)
    - [Pro Guitar/Bass 22-Fret Notes](#pro-guitarbass-22-fret-notes)
  - [Dance Track](#dance-track)
    - [Dance Notes](#dance-notes)
  - [Events Track](#events-track)
  - [Venue Track](#venue-track)
  - [Beat Track](#beat-track)
- [Documentation Notes](#documentation-notes)

## Basic Infrastructure

This only goes over the basic infrastructure of MIDI data, there are plenty of resources elsewhere to learn about the details for the MIDI format itself.

### Chunks

MIDI files are comprised of chunks. There are two types of chunks: header chunks, which contain metadata about the MIDI file itself, such as the format type and the tick resolution, and track chunks, which contain MIDI data streams.

### Events

Track chunks contain a series of MIDI events (also referred to as messages). Events have a delta-time relative to the previous event in the stream, and data related to the event.

### Bytes

Events are a specific set of bytes. Bytes are split evenly into two categories: data bytes (`0x00`-`0x7F`), and status bytes (`0x80`-`0xFF`). As such, you (typically) cannot have a data byte that has a value above `0x7F`, and you cannot have a status byte with a value lower than `0x80`. (The former can be violated in some System Exclusive events, as mentioned later on.)

The specific formats of events is a little beyond the scope of this guide, so they are not covered here.

### Event Categories

There are three categories of events: Channel, System, and Meta.

- Channel events are encoded with a 4-bit channel number (0-15). There are two types of channel events: Voice and Mode.
  - Voice events control a channel/instrument's voices.
  - Mode events control a channel/instrument's response to voice events.
- System events are not encoded with a channel number. There are three types of system events: Common, Real-Time, and Exclusive (SysEx).
  - Common events are received on all channels. These don't particularly pertain to chart files, so no specifics will be given on available events.
  - Real-time events are used for real-time synchronization. These don't particularly pertain to chart files, so no specifics will be given on available events.
  - Exclusive (SysEx) events are flexible, and aside from the starting and ending bytes (`0xF0` and `0xF7` respectively), their format is vendor-defined.
    - They can contain any number of data bytes.
    - Data bytes cannot contain values above `0x7F`, but some SysEx events contain these values anyways. This is the case with the guitar tap note SysEx event, for example.
- Meta events are events that store non-MIDI data such as text.

#### Event Types

Most of these don't matter for charts in most cases, but are listed anyways as part of a general overview.

Channel Voice:

- Note On
  - Marks the start point of a note.
  - Has a note number (0-127) and velocity value (0-127).
  - A Note On with a velocity of 0 is equivalent to a Note Off.
- Note Off
  - Marks the end point of a note.
  - Has a note number (0-127) and velocity value (0-127).
- Control Change
  - Changes the value of a control channel, such as sustain or pitch bend.
  - Has a controller number (0-119) and a control value (0-127).
  - The values for controllers 0-31 and controllers 32-63 are paired up as most-significant bytes and least significant bytes respectively.
    - For example, controller 2 and 34 are each part of the same value, where 2's value is the most significant byte, and 34's value is the least significant byte.
  - Controller numbers 120 through 127 are reserved for channel mode events, detailed in the next list.
- Program Change
  - Changes the sound/voicing of the track.
  - Has a program number (0-127).
- Polyphonic Key Pressure
  - Modifies a specific note in some way.
  - Has a note number (0-127) and pressure value (0-127).
- Channel Pressure
  - Modifies all of the notes in the current MIDI channel in some way.
  - Has a pressure value (0-127).
- Pitch Bend Change
  - Special control change that bends the pitch of the note.
  - Has a least-significant byte and most-significant byte (both 0-127, forms a 14-bit number.)
  - Max negative is 0, 0, center is 0, 64 (8,192), max positive is 127, 127 (16,383).

Channel Mode:

These are special control change events, using the reserved 120-127 control channels.

- All Sound Off
  - Disables all notes and sets their volumes to zero. Has controller number 120.
- Reset All Controllers
  - Resets all control channels to whatever their ideal state is. Has controller number 121.
- Local Control
  - Enables or disables a device's local controls. Has controller number 122. Enabled with a value of 127, disabled with a value of 0.
- All Notes Off
  - Turns off all currently active notes. Has controller number 123.
- Omni Off
  - Sets a device to only respond to events on its basic channel. Also turns off all currently active notes when switched. Has controller number 124.
- Omni On
  - Sets a device to respond to events on all channels. Also turns off all currently active notes when switched. Has controller number 125.
- Mono On (Poly Off)
  - Sets a device to monophonic mode. Has controller number 126.
- Poly On (Mono Off)
  - Sets a device to polyphonic mode. Has controller number 127.

Meta-events:

- Sequence Number
  - Specifies the number of a sequence.
- Text Event
  - Any amount of text describing anything.
- Copyright Notice
  - A copyright notice as text.
- Sequence/Track Name
  - The name of the overall sequence or for a specific track.
- Instrument Name
  - A description of what instrumentation should be used for the track.
- Lyric
  - A singular lyric.
  - This is *not* used for lyrics in charts.
- Marker
  - Name of the current point in a sequence, such as a rehearsal letter or section name.
  - This is *not* used for practice mode sections.
- Cue Point
  - A description of an event happening at this point in an accompanying video, stage performance, etc.
- MIDI Channel Prefix
  - Can be used to associate a MIDI channel with all following events, up until the next channel prefix event.
- End of Track
  - Required event that specifies the exact ending point of a track.
- Set Tempo
  - Sets a tempo in microseconds per MIDI quarter note.
- SMPTE Offset
  - Sets an SMPTE time that a track should start at.
- Time Signature
  - Sets a time signature.
- Key Signature
  - Sets a key signature.

#### Warning For SysEx

As stated in the [Events](#events) section, standard SysEx events normally do not allow values above 127 (`0x7F`). However, some SysEx events violate this, such as the guitar tap note event, so this must be accounted for in MIDI parsing.

## Using via ChartTools

WIP (Info such as reading, writing, etc. should go here)

## Track Names

Tracks are identified by their name.

Standard tracks:

- `PART GUITAR` - Lead Guitar
- `PART GUITAR COOP` - Co-op Guitar
- `PART GUITAR GHL` - Guitar Hero Live Guitar
- `PART BASS` - Bass Guitar
- `PART BASS GHL` - Guitar Hero Live Bass
- `PART RHYTHM` - Rhythm Guitar
- `PART KEYS` - 5-lane Keys
- `PART DRUMS` - Drums/Pro Drums/5-lane Drums
- `PART VOCALS` - Vocals (used for lyrics in CH)
- `EVENTS` - Global events

Additional tracks (from either Rock Band or Phase Shift):

- `PART REAL_GUITAR` - RB3 Pro Guitar (17-fret)
- `PART REAL_GUITAR_22` - RB3 Pro Guitar (22-fret)
- `PART REAL_GUITAR_BONUS` - a
- `PART REAL_BASS` - RB3 Pro Bass (17-fret)
- `PART REAL_BASS_22` - RB3 Pro Bass (22-fret)
- `PART REAL_DRUMS_PS` - PS Drums Real
- `PART REAL_KEYS_X` - RB3 Pro Keys Expert
- `PART REAL_KEYS_M` - RB3 Pro Keys Medium
- `PART REAL_KEYS_H` - RB3 Pro Keys Hard
- `PART REAL_KEYS_E` - RB3 Pro Keys Easy
- `PART KEYS_ANIM_LH` - RB3 Pro Keys left-hand animations
- `PART KEYS_ANIM_RH` - RB3 Pro Keys right-hand animations
- `PART REAL_KEYS_PS_X` - PS Keys Real Expert
- `PART REAL_KEYS_PS_M` - PS Keys Real Medium
- `PART REAL_KEYS_PS_H` - PS Keys Real Hard
- `PART REAL_KEYS_PS_E` - PS Keys Real Easy
- `PART DANCE` - PS 4-key dance
- `HARM1` - RB3 Harmony part 1
- `HARM2` - RB3 Harmony part 2
- `HARM3` - RB3 Harmony part 3
- `VENUE` - RB venue camera/lighting effects
- `BEAT` - RB beat track

Legacy/Other tracks:

- `T1 GEMS` - GH1/2-era version of `PART GUITAR`
- `Click` - Alternate `PART GUITAR` track that FoFiX supports.
- `Midi Out` - Same as above.
- `PART DRUM` - Alternate name for `PART DRUMS` that FoFiX supports in its code.
- `PART REAL GUITAR` - `PART REAL_GUITAR` as it appears in FoFiX's code.
- `PART REAL BASS` - `PART REAL_BASS` as it appears in FoFiX's code.
- `PART REAL DRUM` - Pro Drums as it appears in FoFiX's code.

## Track Details

This section contains lists of MIDI notes, text events, and SysEx events that lay out the chart itself. Not all of the tracks detailed are available in Clone Hero, this is a broader documentation of all known .mid tracks.

**PLEASE NOTE THAT THERE IS ALMOST CERTAINLY STUFF IN RB1/2 CHARTS THAT ARE NOT DOCUMENTED HERE!** Most of this info is based off of the [C3/Rock Band Network docs](http://docs.c3universe.com/rbndocs/index.php?title=Authoring), which has a focus on RB3.

SysEx events follow this format, for the most part:

`50 53 00 00 <difficulty> <type> <enable/disable>`

- `50 53` is the hexadecimal ASCII representation of the letters `PS`, which stands for Phase Shift.
- `00 00` is a constant.
- `Difficulty` is the difficulty this event affects as a hexadecimal number, where Easy is `00`, and Expert is `03`. `FF` means it affects all difficulties.
- `Type` is the event type code. These will be specified later on where relevant.
- `Enable/disable` is a boolean (`00` or `01`) that sets whether open note parsing should be enabled or disabled from this point onward.

### 5-Fret Tracks

These are standard 5-fret tracks that are playable on a guitar controller.

- `PART GUITAR` - Lead Guitar
- `PART GUITAR COOP` - Co-op Guitar
- `PART BASS` - Bass Guitar
- `PART RHYTHM` - Rhythm Guitar
- `PART KEYS` - 5-Lane Keys
  - 5-lane keys gets its own section later on because while CH just treats 5-lane keys as another 5-fret track, RB3 most likely does not expect/allow some of the notes listed here.

#### 5-Fret Notes

Gems/markers:

- 127 - Trill Lane Marker
  - Only applies to Expert unless velocity is between 50 and 41, then it will show up on Hard as well
- 126 - Tremolo Lane Marker
  - Only applies to Expert unless velocity is between 50 and 41, then it will show up on Hard as well
- 124 - Big Rock Ending Marker 1
- 123 - Big Rock Ending Marker 2
- 122 - Big Rock Ending Marker 3
- 121 - Big Rock Ending Marker 4
- 120 - Big Rock Ending Marker 5
  - All 5 must be used as a chord along with a `[coda]` event on the EVENTS track at the start of the chord to initiate a Big Rock Ending.
- 116 - Star Power/Overdrive Marker
- 106 - Score Duel Player 2 Phrase
  - Marks a phrase for RB1/2's Score Duel. (Unsure what exactly they do.)
- 105 - Score Duel Player 1 Phrase
  - Marks a phrase for RB1/2's Score Duel. (Unsure what exactly they do.)
- 104 - Tap Note Marker*
  - *Clone Hero only.
- 103 - Solo Marker, or Star Power if no 116 notes exist (for legacy compatibility)
- ==========
- 102 - Expert Force Strum
- 101 - Expert Force HOPO
- 100 - Expert Orange
- 99  - Expert Blue
- 98  - Expert Yellow
- 97  - Expert Red
- 96  - Expert Green
- 95  - Expert Open*
  - *Only enabled if there's an `[ENHANCED_OPENS]`/`ENHANCED_OPENS` text event at the start.
- ==========
- 90  - Hard Force Strum
- 89  - Hard Force HOPO
- 88  - Hard Orange
- 87  - Hard Blue
- 86  - Hard Yellow
- 85  - Hard Red
- 84  - Hard Green
- 83  - Hard Open*
  - *Only enabled if there's an `[ENHANCED_OPENS]`/`ENHANCED_OPENS` text event at the start.
- ==========
- 78  - Medium Force Strum
- 77  - Medium Force HOPO
- 76  - Medium Orange
- 75  - Medium Blue
- 74  - Medium Yellow
- 73  - Medium Red
- 72  - Medium Green
- 71  - Medium Open*
  - *Only enabled if there's an `[ENHANCED_OPENS]`/`ENHANCED_OPENS` text event at the start.
- ==========
- 66  - Easy Force Strum
- 65  - Easy Force HOPO
- 64  - Easy Orange
- 63  - Easy Blue
- 62  - Easy Yellow
- 61  - Easy Red
- 60  - Easy Green
- 59  - Easy Open**
  - **Only enabled if there's an `[ENHANCED_OPENS]`/`ENHANCED_OPENS` text event at the start. Otherwise, part of the left hand position animation data below.

Animation:

- 59  - Animation - Left Hand Position Highest
- 58  - Animation - Left Hand Position
- 57  - Animation - Left Hand Position
- 56  - Animation - Left Hand Position
- 55  - Animation - Left Hand Position
- 54  - Animation - Left Hand Position
- 53  - Animation - Left Hand Position
- 52  - Animation - Left Hand Position
- 51  - Animation - Left Hand Position
- 50  - Animation - Left Hand Position
- 49  - Animation - Left Hand Position
- 48  - Animation - Left Hand Position
- 47  - Animation - Left Hand Position
- 46  - Animation - Left Hand Position
- 45  - Animation - Left Hand Position
- 44  - Animation - Left Hand Position
- 43  - Animation - Left Hand Position
- 42  - Animation - Left Hand Position
- 41  - Animation - Left Hand Position
- 40  - Animation - Left Hand Position Lowest

Single notes get naturally forced as a HOPO if they are within a 1/12th step of the next note (this can be changed using the `hopo_frequency` song.ini tag), and are not the same color as the previous note or (.mid only) a note in the preceding chord. For a 480-resolution .mid, this would be 160 ticks.

A 1/12th step is also the sustain cutoff threshold: any sustains shorter than this will be cut off and turned into a normal note. This can be changed using the `sustain_cutoff_threshold` song.ini tag.

#### 5-Fret SysEx Events

Open Notes: `50 53 00 00 <difficulty> 01 <enable/disable>`

Tap Notes: `50 53 00 00 FF 04 <enable/disable>`

#### 5-Fret Text Events

Clone Hero:

- `[ENHANCED_OPENS]`/`ENHANCED_OPENS` - Enables note-based open note marking.

Hand maps:

- `[map <handmap>]` - Specifies a handmap to use from this point onwards. `handmap` is an identifier for which handmap to use.
  - Left-hand maps:
    - `HandMap_Default` - Single notes = single-note fingerings, sustains = vibrato, chords = multiple fingers.
    - `HandMap_NoChords` - No chord fingerings.
    - `HandMap_AllChords` - Only chord fingerings.
    - `HandMap_AllBend` - "All ring finger hi vibrato" [sic]
    - `HandMap_Solo` - Dmaj chord fingering for all chords, vibrato for all chord sustains.
    - `HandMap_DropD` - Open-fretting (no fingers down) for all green gems, all other gems are chords.
    - `HandMap_DropD2` - Open-fretting (no fingers down) for all green gems.
    - `HandMap_Chord_C` - C chord fingering for all notes.
    - `HandMap_Chord_D` - D chord fingering for all notes.
    - `HandMap_Chord_A` - A chord fingering for all notes.
  - Right-hand Hand maps (Bass only):
    - `StrumMap_Default` - Finger strumming.
    - `StrumMap_Pick` - Pick strumming/
    - `StrumMap_SlapBass` - Slap strumming.

Animation:

- `[idle]` - Character idles during a part with no notes
- `[idle_realtime]` - Character idles in real-time (not synced to the beat).
- `[idle_intense]` - Character idles intensely.
- `[play]` - Character starts playing.
- `[play_solo]` - Character plays fast while showing off.
- `[mellow]` - Character plays in a mellow manner.
- `[intense]` - Character plays in an intense manner.

### 6-Fret Tracks

These are 6-fret tracks that are playable on a Guitar Hero Live guitar.

- `PART GUITAR GHL` - 6-Fret Lead Guitar
- `PART BASS GHL` - 6-Fret Bass Guitar

TODO: Reference GHL/GHTV charts

#### 6-Fret Notes

Gems/markers:

- 116 - Star Power/Overdrive Marker
- 104 - Tap Note Marker*
  - *Clone Hero only.
- 103 - Solo Marker
- ==========
- 102 - Expert Force Strum
- 101 - Expert Force HOPO
- 100 - Expert Black 3
- 99  - Expert Black 2
- 98  - Expert Black 1
- 97  - Expert White 3
- 96  - Expert White 2
- 95  - Expert White 1
- 94  - Expert Open
- ==========
- 90  - Hard Force Strum
- 89  - Hard Force HOPO
- 88  - Hard Black 3
- 87  - Hard Black 2
- 86  - Hard Black 1
- 85  - Hard White 3
- 84  - Hard White 2
- 83  - Hard White 1
- 82  - Hard Open
- ==========
- 78  - Medium Force Strum
- 77  - Medium Force HOPO
- 76  - Medium Black 3
- 75  - Medium Black 2
- 74  - Medium Black 1
- 73  - Medium White 3
- 72  - Medium White 2
- 71  - Medium White 1
- 70  - Medium Open
- ==========
- 66  - Easy Force Strum
- 65  - Easy Force HOPO
- 64  - Easy Black 3
- 63  - Easy Black 2
- 62  - Easy Black 1
- 61  - Easy White 3
- 60  - Easy White 2
- 59  - Easy White 1
- 58  - Easy Open

#### 6-Fret SysEx Events

Open Notes: `50 53 00 00 <difficulty> 01 <enable/disable>` (redundant, but still supported by CH)

Tap Notes: `50 53 00 00 FF 04 <enable/disable>`

### Drums Tracks

These are the tracks for drums.

- `PART DRUMS` - Standard 4-Lane, 4-Lane Pro, and 5-Lane Drums
- `PART REAL_DRUMS_PS` - Phase Shift's Drums Real

#### Drums Notes

Gems/markers:

- 127 - 2-Lane (Special) Roll Marker
  - Only applies to Expert unless velocity is between 50 and 41, then it will show up on Hard as well
- 126 - 1-Lane (Standard) Roll Marker
  - Only applies to Expert unless velocity is between 50 and 41, then it will show up on Hard as well
- 124 - Big Rock Ending/Fill Marker 1
- 123 - Big Rock Ending/Fill Marker 2
- 122 - Big Rock Ending/Fill Marker 3
- 121 - Big Rock Ending/Fill Marker 4
- 120 - Big Rock Ending/Fill Marker 5
  - All 5 must be used as a chord to mark an SP activation fill, along with a `[coda]` event on the EVENTS track if this is to initiate a Big Rock Ending instead.
- 116 - Star Power/Overdrive Marker
- 112 - Green Tom Marker
- 111 - Blue Tom Marker
- 110 - Yellow Tom Marker
- 109 - Flam Marker
  - In CH, this will turn a note into a RB-style flam:
    - R -> RY
    - Y -> YB
    - B -> YG
    - G -> BG
- 106 - Score Duel Player 2 Phrase
  - Marks a phrase for RB1/2's Score Duel. (Unsure what exactly they do.)
- 105 - Score Duel Player 1 Phrase
  - Marks a phrase for RB1/2's Score Duel. (Unsure what exactly they do.)
- 103 - Solo Marker
- ==========
- 101 - Expert 5-Lane Orange
- 100 - Expert 4-Lane Green/5-Lane Orange
- 99  - Expert Blue
- 98  - Expert Yellow
- 97  - Expert Red
- 96  - Expert Kick
- 95  - Expert+ Kick
- ==========
- 89  - Hard 5-Lane Orange
- 88  - Hard 4-Lane Green/5-Lane Orange
- 87  - Hard Blue
- 86  - Hard Yellow
- 85  - Hard Red
- 84  - Hard Kick
- ==========
- 77  - Medium 5-Lane Orange
- 76  - Medium 4-Lane Green/5-Lane Orange
- 75  - Medium Blue
- 74  - Medium Yellow
- 73  - Medium Red
- 72  - Medium Kick
- ==========
- 65  - Easy 5-Lane Orange
- 64  - Easy 4-Lane Green/5-Lane Orange
- 63  - Easy Blue
- 62  - Easy Yellow
- 61  - Easy Red
- 60  - Easy Kick

Animation (likely not typically present on `PART REAL_DRUMS_PS`):

- 51  - Animation - Floor Tom Right Hand
- 50  - Animation - Floor Tom Left Hand
- 49  - Animation - Tom 2 Right Hand
- 48  - Animation - Tom 2 Left Hand
- 47  - Animation - Tom 1 Right Hand
- 46  - Animation - Tom 1 Left Hand
- 45  - Animation - Crash 2 Soft Left Hand
- 44  - Animation - Crash 2 Hard Left Hand
- 43  - Animation - Ride Cymbal Left Hand
- 42  - Animation - Ride Cymbal Right Hand
- 41  - Animation - Crash 2 Choke
- 40  - Animation - Crash 1 Choke
- 39  - Animation - Crash 2 Soft Right Hand
- 38  - Animation - Crash 2 Hard Right Hand
- 37  - Animation - Crash 1 Soft Right Hand
- 36  - Animation - Crash 1 Hard Right Hand
- 35  - Animation - Crash 1 Soft Left Hand
- 34  - Animation - Crash 1 Hard Left Hand
- 32  - Animation - Other Percussion Right Hand
- 31  - Animation - Hi-Hat Right Hand
- 30  - Animation - Hi-Hat Left Hand
- 29  - Animation - Snare Soft Right Hand
- 28  - Animation - Snare Soft Left Hand
- 27  - Animation - Snare Hard Right Hand
- 26  - Animation - Snare Hard Left Hand
- 25  - Animation - Hi-Hat Open
- 24  - Animation - Kick Right Foot

Additional Modifications:

- A note at a velocity of 127 is an accent note, and a note at a velocity of 1 is a ghost note, though ghost/accent kicks are not supported by any chart editors other than Editor on Fire currently.
  - Clone Hero requires a `[ENABLE_CHART_DYNAMICS]`/`ENABLE_CHART_DYNAMICS` text event to be present in order to parse ghosts/accents, in order to preserve full compatibility with RB charts where velocity doesn't matter in most cases.

#### Drums Real SysEx Events

- Rimshot: `50 53 00 00 <difficulty> 07 <enable/disable>`
- Hi-Hat Open: `50 53 00 00 <difficulty> 05 <enable/disable>`
- Hi-Hat Pedal: `50 53 00 00 <difficulty> 06 <enable/disable>`
- Hi-Hat Sizzle: `50 53 00 00 <difficulty> 08 <enable/disable>`
- Yellow Cymbal + Tom: `50 53 00 00 <difficulty> 11 <enable/disable>`
- Blue Cymbal + Tom: `50 53 00 00 <difficulty> 12 <enable/disable>`
- Green Cymbal + Tom: `50 53 00 00 <difficulty> 13 <enable/disable>`

#### Drums Text Events

Mix events:

- `[mix <difficulty> drums<configuration><flag>]` - Sets a stem configuration for drums.
  - `difficulty` is a number indicating the difficulty this mix event applies to.
    - 0 - Easy
    - 1 - Medium
    - 2 - Hard
    - 3 - Expert
  - `configuration` is a number corresponding to a specific stem configuration for Rock Band.
    - 0 - One stereo stem for the entire kit.
    - 1 - Mono kick, mono snare, stereo kit.
    - 2 - Mono kick, stereo snare, stereo kit.
    - 3 - Stereo kick, stereo snare, stereo kit.
    - 4 - Mono kick, stereo kit.
  - `flag` is an optional additional string added to the end of the mix event that does some form of modification outside of setting up the mix:
    - `d` - Disco Flip. On standard 4-lane, moves the snare stem to be activated by yellow and makes red activate the kit stem. On Pro Drums, flips yellow cymbal/yellow tom and red notes to restore proper playing of the part with cymbals.
    - `dnoflip` - Used in sections where the snare stem should be activated by yellow and the kit stem should be activated by red regardless of if it's Pro or standard 4-lane.
    - `easy` - Used in sections where there are no tom or cymbal gems to unmute the kit stem. Not supported in RB3, the game detects this automatically.
    - `easynokick` - Used in sections where there are no kick gems to unmute the kick stem. Not supported in RB3, the game detects this automatically.

Animation:

- `[idle]` - Character idles during a part with no notes
- `[idle_realtime]` - Character idles in real-time (not synced to the beat).
- `[idle_intense]` - Character idles intensely.
- `[play]` - Character starts playing.
- `[mellow]` - Character plays in a mellow manner.
- `[intense]` - Character plays in an intense manner.
- `[ride_side_<enable>]` - Character uses a side-swipe to hit the ride. `enable` is either `true` or `false`.

#### Drums Additional Notes

The same track is used for drums regardless of whether or not it's standard 4-lane, 4-lane Pro, or 5-lane. Detecting the type of drums track can can be done through checking for tom flags for Pro Drums and checking for 5-lane Green for 5-lane, and falling back to standard 4-lane if neither are present. There are also tags in the song.ini to force what drums should be parsed as, `pro_drums = True` and `five_lane_drums = True`, if the song is such that there *cannot* be either tom flags or 5-lane Green.

### Vocals Tracks

These are the vocals tracks.

- `PART VOCALS` - Standard vocals track
- `HARM1` - Harmonies track 1
- `HARM2` - Harmonies track 2
- `HARM3` - Harmonies track 3

#### Vocals Notes

- 116 - Star Power/Overdrive Marker
  - Standard vocals and Harmonies can have independent overdrive. `HARM2` and `HARM3` get their overdrive from `HARM1`.
- 106 - Lyrics Phrase Marker 2/Score Duel Player 2 Phrase
  - Marks the duration of a lyrics phrase and a phrase for RB1/2's Score Duel. This can appear with the other phrase marker at the same time.
- 105 - Lyrics Phrase Marker 1/Score Duel Player 1 Phrase
  - Marks the duration of a lyrics phrase and a phrase for RB1/2's Score Duel. This can appear with the other phrase marker at the same time.
  - The `HARM1` phrase is used for all 3 harmony tracks. The `HARM2` phrase is only used for when harmony 2/3 lyrics shift in static vocals. Not used in `HARM3`.
- ==========
- 97  - Not Displayed Percussion
- 96  - Displayed Percussion
- ==========
- 84  - C6 (Highest)
  - Does not show up correctly in Rock Band Blitz.
- 83  - B5
- 82  - Bb5
- 81  - A5
Animation:

- `[idle]` - Character idles during a part with no notes
- `[idle_realtime]` - Character idles in real-time (not synced to the beat).
- `[idle_intense]` - Character idles intensely.
- `[play]` - Character starts playing.
- `[mellow]` - Character plays in a mellow manner.
- `[intense]` - Character plays in an intense manner.
- `[ride_side_<enable>]` - Character uses a side-swipe to hit the ride. `enable` is either `true` or `false`.

- 80  - G#5
- 79  - G5
- 78  - F#5
- 77  - F5
- 76  - E5
- 75  - Eb5
- 74  - D5
- 73  - C#5
- 72  - C5
- 71  - B4
- 70  - Bb4
- 69  - A4
- 68  - G#4
- 67  - G4
- 66  - F#4
- 65  - F4
- 64  - E4
- 63  - Eb4
- 62  - D4
- 61  - C#4
- 60  - C4
- 59  - B3
- 58  - Bb3
- 57  - A3
- 56  - G#3
- 55  - G3
- 54  - F#3
- 53  - F3
- 52  - E3
- 51  - Eb3
- 50  - D3
- 49  - C#3
- 48  - C3
- 47  - B2
- 46  - Bb2
- 45  - A2
- 44  - G#2
- 43  - G2
- 42  - F#2
- 41  - F2
- 40  - E2
- 39  - Eb2
- 38  - D2
- 37  - C#2
- 36  - C2 (Lowest)
- ==========
- 1   - Lyric Shift
  - Sets additional shift points for static lyrics.
- 0   - Range Shift
  - Allows the note display range to shift if a vocal range changes drastically.
  - Length determines the speed of the shift, not the duration that it should be shifted for.

#### Vocals Lyrics

Lyrics are stored as text events paired up with the notes in the C1 to C5 range. There are various symbols used for specific things in Rock Band:

- Hyphens `-` indicate that a syllable should be combined with the next.
- Pluses `+` will connect the previous note and the current note into a slide. These are stripped out by CH.
- Equals `=` indicate that a syllable should be joined with the next using a literal hyphen.
- Various symbols are used to mark a note as non-pitched. These are commonly referred to as talkies.
  - Pounds `#` are the standard talkie symbol.
  - Carets `^` have a more generous scoring in RB, typically used on short syllables or syllables without sharp attacks.
  - Asterisks `*` are also used for them in some cases. These are undocumented in the RBN docs.
  - All of these are stripped out by CH.
- Percents `%` are a range divider marker for vocals parts with large octave ranges. These are stripped out by CH.
- Sections `§` are used in Spanish (and likely other languages) lyric authoring to indicate that two syllables are sung as one. These are replaced by a space in CH, and with a tie character `‿` in RB.
- Dollars `$` are used in harmonies to tell the syllables they are part of to hide. These are stripped out by CH.
  - In one case it is also on the standard Vocals track for some reason, it appears to not do anything in RB here.
- Slashes `/` appear in some charts for some reason, mainly The Beatles: Rock Band. These are stripped out by CH.
- Syllables not joined together through anything will automatically be separated by a space.

CH-specific stuff:

- Underscores `_` are replaced with a space by CH.
- CH's PTB can properly parse [TextMeshPro formatting tags](http://digitalnativestudios.com/textmeshpro/docs/rich-text/), such as `<color=#00FF00>`, and has [a whitelist](https://strikeline.myjetbrains.com/youtrack/issue/CH-226) for tags that it allows. Any tags not matching this whitelist will be stripped out. CH v.23 or earlier will break ones that include symbols that get stripped out.

#### Vocals Text Events

Animation:

- `[idle]` - Character idles during a part with no notes
- `[idle_realtime]` - Character idles in real-time (not synced to the beat).
- `[play]` - Character starts playing.
- `[mellow]` - Character plays in a mellow manner.
- `[intense]` - Character plays in an intense manner.
- `[tambourine_start]` - Character plays a tambourine.
- `[tambourine_end]` - Character returns from tambourine to vocals.
- `[cowbell_start]` - Character plays a cowbell.
- `[cowbell_end]` - Character returns from cowbell to vocals.
- `[clap_start]` - Character does claps to the beat.
- `[clap_end]` - Character returns from claps to vocals.

### 5-Lane Keys Track (As Rock Band 3 Expects)

This is (most likely) what Rock Band 3 expects from a Keys track.

- `PART KEYS` - 5-Lane Keys

#### 5-Lane Keys Notes

Gems/markers:

- 127 - Trill Lane Marker
- 124 - Big Rock Ending Marker 1
- 123 - Big Rock Ending Marker 2
- 122 - Big Rock Ending Marker 3
- 121 - Big Rock Ending Marker 4
- 120 - Big Rock Ending Marker 5
  - All 5 must be used along with a `[coda]` event on the EVENTS track.
- 116 - Star Power/Overdrive Marker
- 103 - Solo Marker
- ==========
- 100 - Expert Orange
- 99  - Expert Blue
- 98  - Expert Yellow
- 97  - Expert Red
- 96  - Expert Green
- ==========
- 88  - Hard Orange
- 87  - Hard Blue
- 86  - Hard Yellow
- 85  - Hard Red
- 84  - Hard Green
- ==========
- 76  - Medium Orange
- 75  - Medium Blue
- 74  - Medium Yellow
- 73  - Medium Red
- 72  - Medium Green
- ==========
- 64  - Easy Orange
- 63  - Easy Blue
- 62  - Easy Yellow
- 61  - Easy Red
- 60  - Easy Green

#### 5-Lane Keys Text Events

Animation:

- `[idle]` - Character idles during a part with no notes
- `[idle_realtime]` - Character idles in real-time (not synced to the beat).
- `[idle_intense]` - Character idles intensely.
- `[play]` - Character starts playing.
- `[mellow]` - Character plays in a mellow manner.
- `[intense]` - Character plays in an intense manner.

### Pro Keys Tracks

These are the tracks for Rock Band 3's Pro Keys.

- `PART REAL_KEYS_X` - Pro Keys Expert
- `PART REAL_KEYS_H` - Pro Keys Hard
- `PART REAL_KEYS_M` - Pro Keys Medium
- `PART REAL_KEYS_E` - Pro Keys Easy

#### Pro Keys Notes

TODO

#### Pro Keys Animation Notes

These are for the Pro Keys animation tracks.

- `PART KEYS_ANIM_LH` - Character Keys Left Hand Animations
- `PART KEYS_ANIM_RH` - Character Keys Right Hand Animations

### Keys Real Tracks

These are for Phase Shift's Real Keys mode.

- `PART REAL_KEYS_PS_X` - Real Keys Expert
- `PART REAL_KEYS_PS_M` - Real Keys Hard
- `PART REAL_KEYS_PS_H` - Real Keys Medium
- `PART REAL_KEYS_PS_E` - Real Keys Easy

#### Keys Real Notes

TODO

### Pro Guitar/Bass Tracks

These are the tracks for Pro Guitar and Pro Bass.

- `PART REAL_GUITAR` - Pro Guitar (17-Fret)
- `PART REAL_GUITAR_22` - Pro Guitar (22-Fret)
- `PART REAL_GUITAR_BONUS` - Pro Guitar (?)
- `PART REAL_BASS` - Pro Bass (17-Fret)
- `PART REAL_BASS_22` - Pro Bass (22-Fret)

#### Pro Guitar/Bass 17-Fret Notes

TODO

#### Pro Guitar/Bass 22-Fret Notes

TODO

### Dance Track

This track is a 4-lane Dance mode similar to that of Dance Dance Revolution, In The Groove, and StepMania.

- `PART DANCE` - Dance

#### Dance Notes

- 116 - Star Power/Overdrive Marker
- 103 - Solo Marker
- ==========
- 99  - Challenge Right
- 98  - Challenge Up
- 97  - Challenge Down
- 96  - Challenge Left
- ==========
- 87  - Hard Right
- 86  - Hard Up
- 85  - Hard Down
- 84  - Hard Left
- ==========
- 75  - Medium Right
- 74  - Medium Up
- 73  - Medium Down
- 72  - Medium Left
- ==========
- 63  - Easy Right
- 62  - Easy Up
- 61  - Easy Down
- 60  - Easy Left
- ==========
- 51  - Beginner Right
- 50  - Beginner Up
- 49  - Beginner Down
- 48  - Beginner Left

### Events Track

The Events track contains text events that do various things.

#### Events Notes

- 26 - Hi-Hat Practice Mode Assist Sample
- 25 - Snare Practice Mode Assist Sample
- 24 - Kick Practice Mode Assist Sample

These samples play in Practice Mode in RB3 alongside an isolated stem when the speed is set below 100%.

#### Events Common Text Events

Chart events:

- `[end]` - Marks the end point of the song. For Rock Band, it must be the last event in the chart.
- `[coda]` - Starts a Big Rock Ending.

Crowd events:

- `[music_start]` - Sets the crowd audio to an active state.
- `[music_end]` - Sets the crowd audio to an end-of-song state.

- `[crowd_normal]` - Crowd acts in a typical manner.
- `[crowd_mellow]` - Crowd acts in a more mellow manner.
- `[crowd_intense]` - Crowd acts intensely.
- `[crowd_realtime]` - Crowd acts independently of the beat. Default for the beginning of the song.

- `[crowd_clap]` - Crowd will clap along to the beat if the player is doing well. This is the default.
- `[crowd_noclap]` - Disables the crowd clap.

- `[prc_<name>]` or `[section <name>]` - Marks a practice mode section. `<name>` is the name of the practice mode section.
  - There is [an *absolute unit* of a list of sections available for RBN2](rbn2_practice_sections.md). Clone Hero can convert the events from this list into their proper section names, plus some additional ones not available to RBN2.

### Venue Track

TODO

### Beat Track

TODO

## Documentation Notes

Specifications for the MIDI protocol/format itself are available from the MIDI Association here:

- [MIDI 1.0 Protocol Specifications](https://www.midi.org/specifications/midi1-specifications)
- [Standard MIDI File Specifications](https://www.midi.org/specifications/file-format-specifications/standard-midi-files)

A large amount of the specifications for the tracks comes from the [C3/Rock Band Network docs](http://docs.c3universe.com/rbndocs/index.php?title=Authoring).
