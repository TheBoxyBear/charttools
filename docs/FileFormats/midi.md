# MIDI

A MIDI file is a file that stores a MIDI stream. It is a binary format and uses the `.mid` and `.midi` file extensions (`.midi` is not used for charts or supported by CH, however).

Modern MIDI charts originate from Rock Band's MIDI chart format, with some additional functionality added in by Phase Shift and Clone Hero. Older MIDI charts are slightly different and are based on GH1/2's format, but follow similar patterns.

## Table of Contents

- [Basic Infrastructure](#basic-infrastructure)
  - [Chunks](#chunks)
  - [Events](#events)
  - [Bytes](#bytes)
  - [Event Categories](#event-categories)
    - [Event Types](#event-types)
    - [Warning For SysEx](#warning-for-sysex)
- [Using via ChartTools](#using-via-charttools)
- [Notes about Documentation](#notes-about-documentation)
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
    - [Pro Keys Animation Track Notes](#pro-keys-animation-track-notes)
  - [Keys Real Tracks](#keys-real-tracks)
    - [Keys Real Notes](#keys-real-notes)
  - [Pro Guitar/Bass Tracks](#pro-guitarbass-tracks)
    - [Pro Guitar/Bass Notes](#pro-guitarbass-notes)
    - [Pro Guitar/Bass SysEx Events](#pro-guitarbass-sysex-events)
    - [Pro Guitar/Bass Text Events](#pro-guitarbass-text-events)
  - [Dance Track](#dance-track)
    - [Dance Notes](#dance-notes)
  - [Events Track](#events-track)
    - [Events Notes](#events-notes)
    - [Events Common Text Events](#events-common-text-events)
  - [Venue Track](#venue-track)
    - [Venue Notes (RB1/RB2/RBN1)](#venue-notes-rb1rb2rbn1)
    - [Venue Text Events (RB1/RB2/RBN1)](#venue-text-events-rb1rb2rbn1)
      - [Directed Cuts (RB1/RB2/RBN1)](#directed-cuts-rb1rb2rbn1)
      - [Venue Lighting (RB1/RB2/RBN1)](#venue-lighting-rb1rb2rbn1)
      - [Pyrotechnics (RB1/RB2/RBN1)](#pyrotechnics-rb1rb2rbn1)
    - [Venue Notes (RB3/RBN2)](#venue-notes-rb3rbn2)
    - [Venue Text Events (RB3/RBN2)](#venue-text-events-rb3rbn2)
      - [Camera Cuts (RB3/RBN2)](#camera-cuts-rb3rbn2)
      - [Directed Camera Cuts (RB3/RBN2)](#directed-camera-cuts-rb3rbn2)
      - [Post-Processing Effects (RB3/RBN2)](#post-processing-effects-rb3rbn2)
      - [Venue Lighting (RB3/RBN2)](#venue-lighting-rb3rbn2)
      - [Pyrotechnics (RB3/RBN2)](#pyrotechnics-rb3rbn2)
  - [Beat Track](#beat-track)
    - [Beat Notes](#beat-notes)
- [Resources](#resources)

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

## Notes about Documentation

Generally, specific track documentation follows this format:

- Note lists
- SysEx events
- Text events
- Additional notes (if necessary)

Note numbers in the note lists are 0-indexed (0-127), whereas track channel numbers are 1-indexed (1-16).

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

- `PART DRUMS_2X` - RBN 2x Kick drums chart
  - Likely not found in any charts as this is meant for generating separate 1x and 2x kick CON files for RB
- `PART REAL_GUITAR` - RB3 Pro Guitar (17-fret)
- `PART REAL_GUITAR_22` - RB3 Pro Guitar (22-fret)
- `PART REAL_GUITAR_BONUS` - Unsure.
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

- `50 53` is the hexadecimal ASCII representation of the letters `PS`, which stands for Phase Shift. The `00 00` following it is a constant.
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

- Open Notes: `50 53 00 00 <difficulty> 01 <enable/disable>`
- Tap Notes: `50 53 00 00 FF 04 <enable/disable>`

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

- Open Notes: `50 53 00 00 <difficulty> 01 <enable/disable>` (redundant, but still supported by CH)
- Tap Notes: `50 53 00 00 FF 04 <enable/disable>`

### Drums Tracks

These are the tracks for drums.

- `PART DRUMS` - Standard 4-Lane, 4-Lane Pro, and 5-Lane Drums
- `PART DRUMS_2X` - RBN 2x Kick drums chart
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
  - If a flam marker is present on a chord (excluding the presence of kicks), it does nothing to the chord.
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

Notes/markers:

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

Extended sustains are allowed on Keys in RB3.

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

For RB3, up to 4 notes are allowed in a chord. Extended sustains are also allowed.

Gems/markers:

- 127 - Trill Marker
- 126 - Glissando Marker
- 120 - Big Rock Ending Marker
- 116 - Star Power/Overdrive Marker
  - Only SP/OD placed in the Expert track will work. For RB3, this SP/OD must match with `PART KEYS`.
- 115 - Solo Marker
- ==========
- 72  - C3 (Highest Note)
- 71  - B2
- 70  - A#2
- 69  - A2
- 68  - G#2
- 67  - G2
- 66  - F#2
- 65  - F2
- 64  - E2
- 63  - D#2
- 62  - D2
- 61  - C#2
- 60  - C2
- 59  - B1
- 58  - A#1
- 57  - A1
- 56  - G#1
- 55  - G1
- 54  - F#1
- 53  - F1
- 52  - E1
- 51  - D#1
- 50  - D1
- 49  - C#1
- 48  - C1 (Lowest Note)
- ==========
- 9   - A1-C4 Range
  - The range shift notes will shift the range of the keys shown on the track. In RB3, only a 10th interval (or 17 keys) are shown at a time.
  - These notes do not last the duration of the shift, they simply mark when the shift happens.
  - The A2-C4, F2-A3, and C2-E3 ranges are the main recommended ones, as they are easiest to read in-game.
  - None of the in-between notes to the notes listed here will do range shifts.
- 7   - G2-B3 Range
- 5   - F2-A3 Range
- 4   - E2-G3 Range
- 2   - D2-F3 Range
- 0   - C2-E3 Range

#### Pro Keys Animation Track Notes

These are for the Pro Keys animation tracks.

- `PART KEYS_ANIM_LH` - Character Keys Left Hand Animations
- `PART KEYS_ANIM_RH` - Character Keys Right Hand Animations

- 72  - C3 (Highest Note)
- 71  - B2
- 70  - A#2
- 69  - A2
- 68  - G#2
- 67  - G2
- 66  - F#2
- 65  - F2
- 64  - E2
- 63  - D#2
- 62  - D2
- 61  - C#2
- 60  - C2
- 59  - B1
- 58  - A#1
- 57  - A1
- 56  - G#1
- 55  - G1
- 54  - F#1
- 53  - F1
- 52  - E1
- 51  - D#1
- 50  - D1
- 49  - C#1
- 48  - C1 (Lowest Note)

The character only gets 3 octaves on the keyboard, so the middle octave is shared between left and right hand.

### Keys Real Tracks

These are for Phase Shift's Real Keys mode.

- `PART REAL_KEYS_PS_X` - Real Keys Expert
- `PART REAL_KEYS_PS_M` - Real Keys Hard
- `PART REAL_KEYS_PS_H` - Real Keys Medium
- `PART REAL_KEYS_PS_E` - Real Keys Easy

#### Keys Real Notes

There's no real documentation on how to chart Real Keys, this notes list is taken from a REAPER template for Phase Shift. Some things may be wrong.

Gems/markers:

- 127 - Trill Marker
- 126 - Glissando Marker
- 116 - Star Power/Overdrive Marker
- 115 - Solo Marker
- ==========
- 108 - C8 (Highest Note)
- 107 - B7
- 106 - A#7
- 105 - A7
- 104 - G#7
- 103 - G7
- 102 - F#7
- 101 - F7
- 100 - E7
- 99  - D#7
- 98  - D7
- 97  - C#7
- 96  - C7
- 95  - B6
- 94  - A#6
- 93  - A6
- 92  - G#6
- 91  - G6
- 90  - F#6
- 89  - F6
- 88  - E6
- 87  - D#6
- 86  - D6
- 85  - C#6
- 84  - C6
- 83  - B5
- 82  - A#5
- 81  - A5
- 80  - G#5
- 79  - G5
- 78  - F#5
- 77  - F5
- 76  - E5
- 75  - D#5
- 74  - D5
- 73  - C#5
- 72  - C5
- 71  - B4
- 70  - A#4
- 69  - A4
- 68  - G#4
- 67  - G4
- 66  - F#4
- 65  - F4
- 64  - E4
- 63  - D#4
- 62  - D4
- 61  - C#4
- 60  - C4 (Middle C)
- 59  - B3
- 58  - A#3
- 57  - A3
- 56  - G#3
- 55  - G3
- 54  - F#3
- 53  - F3
- 52  - E3
- 51  - D#3
- 50  - D3
- 49  - C#3
- 48  - C3
- 47  - B2
- 46  - A#2
- 45  - A2
- 44  - G#2
- 43  - G2
- 42  - F#2
- 41  - F2
- 40  - E2
- 39  - D#2
- 38  - D2
- 37  - C#2
- 36  - C2
- 35  - B1
- 34  - A#1
- 33  - A1
- 32  - G#1
- 31  - G1
- 30  - F#1
- 29  - F1
- 28  - E1
- 27  - D#1
- 26  - D1
- 25  - C#1
- 24  - C1
- 23  - B0
- 22  - A#0
- 21  - A0 (Lowest Note)
- ==========
- 13  - Right Hand Range
  - Unsure of how these range markers work.
- 12  - Left Hand Range

### Pro Guitar/Bass Tracks

These are the tracks for Pro Guitar and Pro Bass.

- `PART REAL_GUITAR` - Pro Guitar (17-Fret)
- `PART REAL_GUITAR_22` - Pro Guitar (22-Fret)
- `PART REAL_GUITAR_BONUS` - Pro Guitar (?)
- `PART REAL_BASS` - Pro Bass (17-Fret)
- `PART REAL_BASS_22` - Pro Bass (22-Fret)

#### Pro Guitar/Bass Notes

Gems/markers:

- 127 - Trill Marker
- 126 - Tremolo Marker
- 125 - Big Rock Ending Marker 1
- 124 - Big Rock Ending Marker 2
- 123 - Big Rock Ending Marker 3
- 122 - Big Rock Ending Marker 4
- 121 - Big Rock Ending Marker 5
- 120 - Big Rock Ending Marker 6
- 116 - Star Power/Overdrive Marker
- 115 - Solo Marker
- 108 - Left Hand Position
  - Marks where on the guitar a player's left hand should be, and determines how note numbers get positioned on top of the note.
  - Applies from the current point onward until a new marker is encountered.
  - Position determined by note velocity, starting from velocity 101 and going to velocity 114 for 17-fret or 119 for 22-fret.
- 107 - Force Chord Numbering
- ==========
- 105 - Expert Strum Direction Marker
  - Uses track channel numbers to emphasize a strum direction.
  - Channel 14 - Emphasize the higher strings.
  - Channel 15 - Emphasize the middle strings.
  - Channel 16 - Emphasize the lower strings.
- 104 - Expert Arpeggio Marker
  - Requires a note/chord on track channel 2 to use as a ghost note/chord shape in-game.
- 103 - Expert Slide Marker
  - Velocity 107 - Slide Up
  - Velocity 108 - Slide Down
  - These directions are inconsistent due to some complicated rules.
  - Placing this note on track channel 12 will reverse the direction.
- 102 - Expert Force HOPO
- 101 - Expert Purple (e)
- 100 - Expert Yellow (B)
- 99  - Expert Blue (G)
- 98  - Expert Orange (D)
- 97  - Expert Green (A)
- 96  - Expert Red (E)
- ==========
- 80  - Hard Arpeggio Marker
- 79  - Hard Slide Marker
- 78  - Hard Force HOPO
- 77  - Hard Purple (e)
- 76  - Hard Yellow (B)
- 75  - Hard Blue (G)
- 74  - Hard Orange (D)
- 73  - Hard Green (A)
- 72  - Hard Red (E)
- ==========
- 56  - Medium Arpeggio Marker
- 55  - Medium Slide Marker
- 54  - Medium Force HOPO
  - Likely not supported in RB3.
- 53  - Medium Purple (e)
- 52  - Medium Yellow (B)
- 51  - Medium Blue (G)
- 50  - Medium Orange (D)
- 49  - Medium Green (A)
- 48  - Medium Red (E)
- ==========
- 32  - Easy Arpeggio Marker
- 31  - Easy Slide Marker
- 30  - Easy Force HOPO
  - Likely not supported in RB3.
- 29  - Easy Purple (e)
- 28  - Easy Yellow (B)
- 27  - Easy Blue (G)
- 26  - Easy Orange (D)
- 25  - Easy Green (A)
- 24  - Easy Red (E)
- ==========
- 18  - Sharp/Flat Swap
- 17  - Hide Chord Names Marker
- 16  - Slash Chord Marker
  - Used for chords such as `Fm7/E` or `Am/G`.
- 15  - Root Note Marker (D#/Eb)
- 14  - Root Note Marker (D)
- 13  - Root Note Marker (C#/Db)
- 12  - Root Note Marker (C)
- 11  - Root Note Marker (B)
- 10  - Root Note Marker (A#/Bb)
- 9   - Root Note Marker (A)
- 8   - Root Note Marker (G#/Ab)
- 7   - Root Note Marker (G)
- 6   - Root Note Marker (F#/Gb)
- 5   - Root Note Marker (F)
- 4   - Root Note Marker (E)

Fret numbers:

- Fret number is determined by velocity, starting from velocity 100 and going to velocity 117 for 17-fret or 122 for 22-fret.

Track channels:

- Different channel numbers modify the notes in different ways. Ones specific to a marker are listed in the notes list with that marker.
- 1 - Normal notes and markers
- 2 - Ghost notes
- 3 - Bend notes
- 4 - Muted notes
- 5 - Tapped notes
- 6 - Harmonics
- 7 - Pinch harmonics

Other notes:

- There are separate tracks for 17-fret and 22-fret due to different controller models having a different amount of available frets.
- Root notes are required for chords, but they don't have to be placed on every chord. It's only necessary to place one when the root note changes.
- For RB3, overdrive must match that of standard Guitar/Bass.

#### Pro Guitar/Bass SysEx Events

Phase Shift:

- Slide Up: `50 53 00 00 <difficulty> 02 <enable/disable>`
- Slide Down: `50 53 00 00 <difficulty> 03 <enable/disable>`

#### Pro Guitar/Bass Text Events

RB3 Pro Guitar trainer text events:

- Not usable in RBN2/custom charts, only in custom Pro upgrades:
- `[begin_pg song_trainer_pg_<number>]` - Marks the start point of a Pro Guitar trainer section.
  - `number` is a number index for the trainer, starting at 1.
- `[end_pg song_trainer_pg_<number>]` - Marks the end point of a Pro Guitar trainer section.
  - `number` is a number index for the trainer, starting at 1.
- `[pg_norm song_trainer_pg_<number>]` - Marks the loop point of a Pro Guitar trainer section.
  - `number` is a number index for the trainer, starting at 1.

RB3 Pro Bass trainer text events:

- Not usable in RBN2/custom charts, only in custom Pro upgrades:
- `[begin_pb song_trainer_pb_<number>]` - Marks the start point of a Pro Bass trainer section.
  - `number` is a number index for the trainer, starting at 1.
- `[end_pb song_trainer_pb_<number>]` - Marks the end point of a Pro Bass trainer section.
  - `number` is a number index for the trainer, starting at 1.
- `[pb_norm song_trainer_pb_<number>]` - Enables doing a seamless endless loop of the section.
  - `number` is a number index for the trainer, starting at 1.

Chord names:

- `[chrd<difficulty> <name>]` - Overrides the chord name shown for the current chord.
  - `difficulty` is a number representing the difficulty.
  - `name` is the name to be given to the chord. A `<gtr></gtr>` tag pair can be used for superscript in RB3.

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

Controls camera placement/effects and venue lighting/effects.

Rock Band Network 1 and Rock Band Network 2 have different sets of venue notes and events, so categories are split into RB1/RB2/RBN1 and RB3/RBN2.

#### Venue Notes (RB1/RB2/RBN1)

- 110 - Trails
- 109 - Security Camera
- 108 - Black and White
- 107 - Lines
- 106 - Blue Tint
- 105 - Mirror
- 104 - Bloom B
- 103 - Bloom A
- 102 - Photocopy
- 101 - Negative
- 100 - Silvertone
- 99  - Sepia
- 98  - 16mm
- 97  - Contrast A
- 96  - Default Effect
- ==========
- 87  - Guitarist Sing Along
  - Has the guitarist sing along with the vocalist.
- 86  - Drummer Sing Along
  - Has the drummer sing along with the vocalist.
- 85  - Bassist Sing Along
  - Has the bassist sing along with the vocalist.
- ==========
- 73  - No Close Ups
- 72  - Only Close Ups
- 71  - Only Far Shots
- 70  - No Behind Shots
- 64  - Focus on Vocalist
- 63  - Focus on Guitarist
- 62  - Focus on Drummer
- 61  - Focus on Bassist
- 60  - Camera Cut
  - Randomly switches to a new shot of the band. Works in tandem with the 8 notes above. (Speculation) Not used when calling a directed camera cut.
- 50  - First Keyframe
  - Goes to the first keyframe of a keyframed venue lighting effect.
- 49  - Previous Keyframe
  - Goes to the previous keyframe of a keyframed venue lighting effect.
- 48  - Next Keyframe
  - Goes to the next keyframe of a keyframed venue lighting effect.
- ==========
- 40  - Spotlight on Vocals
- 39  - Spotlight on Guitar
- 38  - Spotlight on Drums
- 37  - Spotlight on Bass

#### Venue Text Events (RB1/RB2/RBN1)

##### Directed Cuts (RB1/RB2/RBN1)

Directed cuts are special camera cuts which use animations that don't appear in the standard looping animations for basic camera cuts. These have a variable amount of pre-roll, so the event is placed where the "hit" of the animation goes: for example, if the guitarist should jump or kick the camera, the event should be placed at the time the guitarist should land, or when the kick hits the camera.

There are two ways to call a directed cut:

- `[do_directed_cut <cut>]` - Always play the cut.
- `[do_optional_cut <cut>]` - Only play the cut if the player is doing well.

`cut` is the cut to be called:

- Full band:
  - `directed_all`
  - `directed_all_yeah`
  - `directed_bre` - Band flails around or is otherwise intense for a BRE.
  - `directed_brej` - Band does an impactful thing like stomp, strum hard, or bang on drums hard on the last BRE note.
  - `directed_all_lt` - Long shot heading away from the stage.
  - `directed_all_cam` - Entire band interacts with camera.
- Individual characters:
  - `directed_guitar`
  - `directed_bass`
  - `directed_drums`
  - `directed_vocals`
- Individual idle characters:
  - `directed_guitar_np`
  - `directed_bass_np`
  - `directed_drums_np`
  - `directed_vocals_np`
- Individual character camera interactions:
  - `directed_guitar_cam`
  - `directed_bass_cam`
  - `directed_drums_cam`
  - `directed_vocals_cam`
- Individual character closeups:
  - `directed_guitar_cls` - Guitar fretboard closeup.
  - `directed_bass_cls` - Bass fretboard closeup.
  - `directed_drums_kd` - Drums kick drum closeup.
  - `directed_vocals_cls`
- Character singalongs:
  - Used in conjunction with the singalong notes.
  - `directed_duo_guitar` - Guitarist sings along and interacts with vocalist.
  - `directed_duo_bass` - Bassist sings along and interacts with vocalist.
  - `directed_duo_drums` - Drummer sings along, no vocalist in shot.
- Miscellaneous cuts:
  - `directed_duo_gb` - Guitar and bass interactions.
  - `directed_drums_lt`- Long shot rotating around the drummer.
  - `directed_drums_pnt` - Drummer points at camera.
  - `directed_stagedive` - Vocalist jumps off stage, camera cuts away once they land.
  - `directed_crowdsurf` - Vocalist jumps off stage, camera cuts to them crowdsurfing.
  - `directed_crowd_g` - Guitarist interacts with crowd.
  - `directed_crowd_b` - Bassist interacts with crowd.

##### Venue Lighting (RB1/RB2/RBN1)

- `[verse]` - Marks a verse of a song. One of these must be placed before any other lighting call events to initialize the lighting system.
- `[chorus]` - Marks a chorus of a song.
- `[lighting (<descriptor>)]` - Sets a venue lighting cue. `descriptor` is an identifier for which cue to set, listed below.
  - Keyframed calls:
    - a
    - `()` - Setting no descriptor will set the lighting to default.
    - `()` + `[verse]` - Default verse lighting.
    - `()` + `[chorus]` - Default chorus lighting.
    - `manual_cool` - Cool-temperature lighting.
    - `manual_warm` - Warm-temperature lighting.
    - `dischord` - Harsh, dissonant lighting.
    - `stomp` - All lights on or off. The Next Keyframe note toggles this on/off.
  - Automatic calls:
    - `loop_cool` - Cool-temperature lighting.
    - `loop_warm` - Warm-temperature lighting.
    - `harmony` - Harmonious lighting.
    - `frenzy` - Frenetic, dissonant lighting.
    - `silhouettes` - Dark, atmospheric lighting; shows darkened silhouettes of the characters.
    - `silhouettes_spot` - Same as above, but characters are slightly visible.
    - `searchlights` - Lights that sweep individually.
    - `sweep` - Lights that sweep together in banks.
    - `strobe_slow` - Strobe light that blinks every 16th note/120 ticks.
    - `strobe_fast` - Strobe light that blinks every 32nd note/60 ticks.
    - `blackout_fast` - Darken the stage quickly. The event should be placed at the point where full darkness is desired. The game engine will automatically fade out from the previous lighting state over a period of 0.2 seconds.
    - `blackout_slow` - Darken the stage slowly. The event should be placed at the point where full darkness is desired. The game engine will automatically fade out from the previous lighting state over a period of 2 seconds. Because of the long fade out, the event will not go into effect if the previous lighting state is placed too close to the position of the event.
    - `flare_slow` - Bright white flare that fades slowly into the next lighting preset.
    - `flare_fast` - Bright white flare that fades quickly into the next lighting preset.
    - `bre` - Frenetic lighting used during a Big Rock Ending. Looks like [lighting (frenzy)], only crazier.

##### Pyrotechnics (RB1/RB2/RBN1)

These events trigger an explosion or flamethrowers on the venue. These only work in arena venues.

- `[bonusfx]` - Triggers an explosion effect.
- `[bonusfx_optional]` - Same as above, but the effect will only be triggered when the player is doing well.

These should not be used during a BRE, as it does this automatically.

#### Venue Notes (RB3/RBN2)

- 87  - Guitarist Sing Along
  - Has the guitarist sing along with the vocalist. Replaced with keyboardist if guitar is absent.
- 86  - Drummer Sing Along
  - Has the drummer sing along with the vocalist.
- 85  - Bassist Sing Along
  - Has the bassist sing along with the vocalist. Replaced with keyboardist if bass is absent.
- ==========
- 41  - Spotlight on Keys
- 40  - Spotlight on Vocals
- 39  - Spotlight on Guitar
- 38  - Spotlight on Drums
- 37  - Spotlight on Bass

#### Venue Text Events (RB3/RBN2)

##### Camera Cuts (RB3/RBN2)

These text events specify a camera shot to be used. Only 4 band members can be on-stage at a time out of the 5 possible instrument types, so camera cuts are often stacked on the same point to ensure a proper shot is used. The camera system has a priority list it uses to pick a shot that most closely matches the characters on-stage. If none of the authored cuts match the available parts, it will pick from a list of generic parts.

These cuts are listed roughly in order from most generic (least priority) to most specific (highest priority).

Four-character cuts:

- `[coop_all_behind]`
- `[coop_all_far]`
- `[coop_all_near]`

Three-character cuts (no drums):

- `[coop_front_behind]`
- `[coop_front_near]`

One-character standard cuts:

- `[coop_d_behind]`
- `[coop_d_near]`
- `[coop_v_behind]`
- `[coop_v_near]`
- `[coop_b_behind]`
- `[coop_b_near]`
- `[coop_g_behind]`
- `[coop_g_near]`
- `[coop_k_behind]`
- `[coop_k_near]`

One-character closeups:

- `[coop_d_closeup_hand]`
- `[coop_d_closeup_head]`
- `[coop_v_closeup]`
- `[coop_b_closeup_hand]`
- `[coop_b_closeup_head]`
- `[coop_g_closeup_hand]`
- `[coop_g_closeup_head]`
- `[coop_k_closeup_hand]`
- `[coop_k_closeup_head]`

Two-character cuts:

- `[coop_dv_near]`
- `[coop_bd_near]`
- `[coop_dg_near]`
- `[coop_bv_behind]`
- `[coop_bv_near]`
- `[coop_gv_behind]`
- `[coop_gv_near]`
- `[coop_kv_behind]`
- `[coop_kv_near]`
- `[coop_bg_behind]`
- `[coop_bg_near]`
- `[coop_bk_behind]`
- `[coop_bk_near]`
- `[coop_gk_behind]`
- `[coop_gk_near]`

##### Directed Camera Cuts (RB3/RBN2)

Directed cuts are special camera cuts which use animations that don't appear in the standard looping animations for basic camera cuts. These are placed where the "hit" of the animation goes: for example, if the guitarist should jump or kick the camera, the event should be placed at the time the guitarist should land, or when the kick hits the camera.

Directed cuts have higher priority than standard camera cuts, and the events listed here are also roughly in priority order.

Full band:

- `[directed_all]`
- `[directed_all_cam]`
- `[directed_all_yeah]`
- `[directed_all_lt]`*
- `[directed_bre]`
- `[directed_brej]`

- `[directed_crowd]`

Single character:

- `[directed_drums]`
- `[directed_drums_pnt]`
- `[directed_drums_np]`
- `[directed_drums_lt]`*
- `[directed_drums_kd]`*
- `[directed_vocals]`
- `[directed_vocals_np]`
- `[directed_vocals_cls]`
- `[directed_vocals_cam_pr]`
- `[directed_vocals_cam_pt]`
- `[directed_stagedive]`
- `[directed_crowdsurf]`
- `[directed_bass]`
- `[directed_crowd_b]`
- `[directed_bass_np]`
- `[directed_bass_cam]`
- `[directed_bass_cls]`*
- `[directed_guitar]`
- `[directed_crowd_g]`
- `[directed_guitar_np]`
- `[directed_guitar_cls]`*
- `[directed_guitar_cam_pr]`
- `[directed_guitar_cam_pt]`
- `[directed_keys]`
- `[directed_keys_cam]`
- `[directed_keys_np]`

Two characters:

- `[directed_duo_drums]`
- `[directed_duo_guitar]`
- `[directed_duo_bass]`
- `[directed_duo_kv]`
- `[directed_duo_gb]`
- `[directed_duo_kb]`
- `[directed_duo_kg]`

*These free directed cut flags only involve unique camera angles while still using looping animation flags authored on the instrument tracks.

##### Post-Processing Effects (RB3/RBN2)

These text events apply post-processing effects to the camera.

Basic post-processing:

- `[ProFilm_a.pp]` - Default, no notable effects
- `[ProFilm_b.pp]` - Slightly mutes colors
- `[video_a.pp]` - Slightly grainy video
- `[film_16mm.pp]` - Grainy video
- `[shitty_tv.pp]` - Very grainy video, dramatically lightens colors
- `[bloom.pp]` - Slightly brightens picture and adds a low-FPS effect
- `[film_sepia_ink.pp]` - Reduces colors to yellowish-gray shades
- `[film_silvertone.pp]` - Reduces colors to gray shades
- `[film_b+w.pp]` - Reduces colors to a smaller range of gray shades than `film_silvertone`
- `[video_bw.pp]` - Reduces colors to a smaller range of gray shades than `film_silvertone`, slightly gritty
- `[contrast_a.pp]` - Very gritty, somewhat polarized black and white
- `[photocopy.pp]` - Choppy, low frame-per-second effect
- `[film_blue_filter.pp]` - Reduces colors to blue shades
- `[desat_blue.pp]` - Produces slightly grainy images with blue tinge
- `[video_security.pp]` - Grainy, reduces colors to green shades

Special post-processing:

- `[bright.pp]` - Brightens lights to a bloom-esque effect, lightens darks
- `[posterize.pp]` - Flattens colors, notable in shadows
- `[clean_trails.pp]` - Creates a small video feed delay, like a visual "echo"
- `[video_trails.pp]` - Creates a video feed delay, longer delay than `clean_trails`
- `[flicker_trails.pp]` - Creates a video feed delay, slightly darkens images and mutes colors
- `[desat_posterize_trails.pp]` - Creates a long video feed delay, flattens colors
- `[film_contrast.pp]` - Darkens darks, lightens lights
- `[film_contrast_blue.pp]` - Darkens darks, lightens lights, slightly blue hues
- `[film_contrast_green.pp]` - Darkens darks, lightens lights slightly green hues
- `[film_contrast_red.pp]` - Darkens darks, lightens lights slightly red hues
- `[horror_movie_special.pp]` - Polarizes colors to either red or black
- `[photo_negative.pp]` - Inverses colors
- `[ProFilm_mirror_a.pp]` - Left side of screen mirrors right side, changes colors to variety of oranges, greens, and yellows
- `[ProFilm_psychedelic_blue_red.pp]` - Polarizes colors to either red or blue
- `[space_woosh.pp]` - Lightens colors dramatically, creates three small video feed delays in blue, red, and green

##### Venue Lighting (RB3/RBN2)

- `[first]` - Goes to the first keyframe of a keyframed venue lighting effect.
- `[prev]` - Goes to the previous keyframe of a keyframed venue lighting effect.
- `[next]` - Goes to the next keyframe of a keyframed venue lighting effect.
- `[lighting (<descriptor>)]` - Sets a venue lighting cue. `descriptor` is an identifier for which cue to set, listed below.
  - `intro` - Transitions lighting from the starting state to the authored lighting.
    - This doesn't seem to be necessary as of RB3.
  - Keyframed cues:
    - These use keyframes to cycle through different colors.
    - `verse` - Soft yet full blends, such as orange and green; varies per venue.
    - `chorus` - Stark, dramatic colors to invoke a peak state, such as saturated blue and red; varies per venue.
    - `manual_cool` - Cool-temperature lighting.
    - `manual_warm` - Warm-temperature lighting.
    - `dischord` - Harsh lighting, blend of dissonant colors.
    - `stomp` - All lights on or off; only responds to the `[next]` trigger.
  - Automatic cues:
    - `loop_cool` - Blend of cool temperature colors.
    - `loop_warm` - Blend of warm temperature colors.
    - `harmony` - Blend of a harmonious color palette.
    - `frenzy` - Frenetic, dissonant colored lighting that alternates quickly.
    - `silhouettes` - Dark, atmospheric lighting, shows darkened silhouettes of characters.
    - `silhouettes_spot` - Same as above, but can be used in conjunction with spotlights. Characters are also visible.
    - `searchlights` - Lights that sweep individually.
    - `sweep` - Lights that sweep together in banks.
    - `strobe_slow` - Strobe light that blinks every 16th note/120 ticks.
    - `strobe_fast` - Strobe light that blinks every 32nd note/60 ticks.
    - `blackout_slow` - Darken the stage slowly.
    - `blackout_fast` - Darken the stage quickly.
    - `blackout_spot` - Blackout state with an added underlighting (the blackout equivalent of `silhouettes_spot`).
    - `flare_slow` - Bright white flare that fades slowly into the next lighting preset.
    - `flare_fast` - Bright white flare that fades quickly into the next lighting preset.
    - `bre` - Frenetic lighting used during a Big Rock Ending. Looks like `frenzy`, only crazier.

`[lighting ()]`, `[verse]`, and `[chorus]` are not valid events for RBN2.

##### Pyrotechnics (RB3/RBN2)

These events trigger an explosion or flamethrowers on the venue. These only work in arena venues.

- `[bonusfx]` - Triggers an explosion effect.
- `[bonusfx_optional]` - Same as above, but the effect will only be triggered when the player is doing well.

These should not be used during a BRE, as it does this automatically.

### Beat Track

The `BEAT` track is used to determine where upbeats and downbeats in the song are separately from the tempo and time signature. In Rock Band, this drives character animations, lighting, the crowd, and how quickly Overdrive depletes.

For RB, the last note in this track must occur one beat before the `[end]` event.

#### Beat Notes

- 13 - Upbeat
- 12 - Downbeat (first beat of a measure)

## Resources

Specifications for the MIDI protocol/format itself are available from the MIDI Association here:

- [MIDI 1.0 Protocol Specifications](https://www.midi.org/specifications/midi1-specifications)
- [Standard MIDI File Specifications](https://www.midi.org/specifications/file-format-specifications/standard-midi-files)

A large amount of the specifications for the tracks comes from the [C3/Rock Band Network docs](http://docs.c3universe.com/rbndocs/index.php?title=Authoring).
