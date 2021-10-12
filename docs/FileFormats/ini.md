# .ini

Initialization files are commonly-used files for storing things such as settings/configurations. They are text-based and use the `.ini` file extension.

Song metadata for .mid files are stored in a song.ini file. Clone Hero also extends this file to .chart files.

## Table of Contents

- [Basic Structure](#basic-structure)
  - [Sections](#sections)
  - [Tags](#tags)
- [Using via ChartTools](#using-via-charttools)
- [Available Tags](#available-tags)
  - [Common Tags](#common-tags)
  - [Legacy/Unused Tags](#legacyunused-tags)
- [Documentation Notes](#documentation-notes)

## Basic Structure

### Sections

A song.ini file should be started off with a `[song]`/`[Song]` section header. Leading or trailing line breaks don't matter for CH.

### Tags

Tags are a key-value pair that store each metadata entry.

`<Key> = <Value>`

`Key` is a string, and `Value` is either a string, number, or a boolean, depending on the key. No specific number types are known so far.

The equals sign may, but is not required to be, padded by spaces, but it is recommended as not all games/programs support song.ini files without the padding (Phase Shift, for example).

## Using via ChartTools

WIP (Info such as reading, writing, etc. should go here)

Any tags not modelled by the Metadata class are added to an UnidentifiedData list.

## Available Tags

There are *many* tags in existence, so listing them all in one place is rather difficult. Still, this should be a pretty good amount of them. Additionally, tags can be in any order in the song.ini, so you should *not* rely on the order of tags to determine what is what. There may also be tags out there with inconsistent capitalization.

Typically, a value of -1 is used in number values to specify that the value does not exist (such as setting a difficulty to -1 to display no difficulty number in-game). Other negative numbers should be treated the same unless the tag explicitly supports a negative value.

### Common Tags

| `Key`                      | Description                                                                                             | Data type | Example |
| :------------------------: | :------------------------------------------------------------------------------------------------------ | :-------: | :-----: |
| `name`                     | Title of the song.                                                                                      | string    | |
| `artist`                   | Artist or band behind the song.                                                                         | string    | |
| `album`                    | Title of the album the song is featured in.                                                             | string    | |
| `genre`                    | Genre of the song.                                                                                      | string    | |
| `year`                     | Year of the song’s release.                                                                             | string    | |
| `album_track`              | Track number of the song within the album it's from.                                                    | number    | |
| `track`                    | (Legacy) Same as `album_track`.                                                                         | number    | |
| `playlist_track`           | Track number of the song within the playlist/setlist it's from.                                         | number    | |
| `charter`                  | Community member responsible for charting the song.                                                     | string    | |
| `frets`                    | (Legacy) Same as `charter`.                                                                             | string    | |
| `playlist`                 | (CH) Playlist that the song should show up in.                                                          | string    | |
| `sub_playlist`             | (CH) Sub-playlist that the song should show up in.                                                      | string    | |
| `diff_guitar`              | Estimated difficulty of the Lead Guitar track.                                                          | number    | |
| `diff_guitarghl`           | Estimated difficulty of the 6 Fret Lead track.                                                          | number    | |
| `diff_bass`                | Estimated difficulty of the Bass Guitar track.                                                          | number    | |
| `diff_bassghl`             | Estimated difficulty of the 6 Fret Bass track.                                                          | number    | |
| `diff_rhythm`              | Estimated difficulty of the Rhythm Guitar track.                                                        | number    | |
| `diff_keys`                | Estimated difficulty of the Keys track.                                                                 | number    | |
| `diff_drums`               | Estimated difficulty of the Drums track.                                                                | number    | |
| `song_length`              | Length of the song's audio in milliseconds.                                                             | number    | |
| `delay`                    | (Not recommended, legacy) Chart delay time in milliseconds. Can be negative. Higher = later start.      | number    | |
| `preview_start_time`       | Timestamp in milliseconds where the song preview starts.                                                | number    | |
| `video_start_time`         | Timestamp in milliseconds where playback of an included video will start. Can be negative.              | number    | |
| `icon`                     | Name of the charter icon to display for this song on the main menu.                                     | string    | |
| `loading_phrase`           | Text that will be shown during instrument/difficulty/modifier selection.                                | string    | |
| `modchart`                 | Indicates if this song is a modchart.                                                                   | boolean   | |
| `pro_drums`                | Forces the Drums track to be read as Pro Drums charted regardless of tom/cymbal markers being detected. | boolean   | |
| `five_lane_drums`          | Forces the Drums track to be read as 5-lane regardless of if 5-lane Green is detected.                  | boolean   | |
| `sustain_cutoff_threshold` | Overrides the default threshold under which a sustain will get cut off. Doesn't work for .chart files.  | number    | |
| `hopo_frequency`           | Overrides the natural HOPO threshold. Doesn't work for .chart files.                                    | number    | |
| `eighthnote_hopo`          | Overrides the natural HOPO threshold from a 1/12th step to a 1/8th step.                                | boolean number (standard bool also works for CH) | |
| `multiplier_note`          | Overrides the SP phrase MIDI note for .mid charts. For CH, must be 116 or 103.                          | number    | |
| `end_events`               | Overrides whether or not end events in the chart will be respected.                                     | boolean   | |

### Uncommon/Legacy/Unsupported Tags

| `Key`                        | Description                                                                          | Data description | Example |
| :--------------------------: | :----------------------------------------------------------------------------------- | :--------------: | :-----: |
| `sub_genre`                  | Sub-genre for the song.                                                              | string           | |
| `lyrics`                     | Indicates if the song has lyrics or not.                                             | boolean          | |
| `diff_band`                  | Estimated average difficulty of all tracks in the song.                              | number           | |
| `diff_guitar_coop`           | Estimated difficulty of the Guitar Co-op track.                                      | number           | |
| `diff_guitar_real`           | Estimated difficulty of the Pro Guitar track.                                        | number           | |
| `diff_guitar_real_22`        | Estimated difficulty of the Pro Guitar 22-fret track.                                | number           | |
| `diff_bass_real`             | Estimated difficulty of the Pro Bass track.                                          | number           | |
| `diff_bass_real_22`          | Estimated difficulty of the Pro Bass 22-fret track.                                  | number           | |
| `diff_drums_real`            | Estimated difficulty of the Pro Drums track.                                         | number           | |
| `diff_drums_real_ps`         | Estimated difficulty of the Drums Real track.                                        | number           | |
| `diff_keys_real`             | Estimated difficulty of the Pro Keys track.                                          | number           | |
| `diff_keys_real_ps`          | Estimated difficulty of the Keys Real track.                                         | number           | |
| `diff_vocals`                | Estimated difficulty of the Vocals track.                                            | number           | |
| `diff_vocals_harm`           | Estimated difficulty of the Harmonies tracks.                                        | number           | |
| `diff_dance`                 | Estimated difficulty of the Dance track.                                             | number           | |
| `preview`                    | Two timestamps in milliseconds for preview start and end time.                       | number[2]        | `55000 85000` |
| `preview_end_time`           | (May not actually exist) Timestamp in milliseconds that the preview should stop at.  | number           | |
| `sysex_slider`               | Enables SysEx events for sliders/tap notes.                                          | boolean          | |
| `sysex_high_hat_ctrl`        | Enables SysEx events for Drums Real hi-hat pedal control.                            | boolean          | |
| `sysex_rimshot`              | Enables SysEx events for Drums Real rimshot hits.                                    | boolean          | |
| `sysex_open_bass`            | Enables SysEx events for open notes.                                                 | boolean          | |
| `sysex_pro_slide`            | Enables SysEx events for Pro Guitar/Bass slide directions.                           | boolean          | |
| `banner_link_a`              | Link that clicking banner A will open.                                               | string           | |
| `link_name_a`                | Name for banner A.                                                                   | string           | |
| `banner_link_b`              | Link that clicking banner B will open.                                               | string           | |
| `link_name_b`                | Name for banner B.                                                                   | string           | |
| `background`                 | (Assumption) Name/path for a background image file.                                  | string           | |
| `video`                      | (Assumption) Name/path for a video file.                                             | string           | |
| `video_end_time`             | Timestamp in milliseconds where playback of an included video background will end.   | number           | |
| `video_loop`                 | Sets whether or not the video should loop.                                           | boolean          | |
| `guitar_type`                | Sample sound set for Guitar.                                                         | number           | |
| `bass_type`                  | Sample sound set for Bass.                                                           | number           | |
| `kit_type`                   | Sample sound set for Drumkit.                                                        | number           | |
| `keys_type`                  | Sample sound set for Keyboard.                                                       | number           | |
| `dance_type`                 | Unsure.                                                                              | number           | |
| `vocal_gender`               | Specifies a voice type for the singer (male or female voice).                        | string           | |
| `real_guitar_tuning`         | Specifies a tuning for 17-fret Pro Guitar.                                           | number[4, 5, or 6], string (optional) | `0 0 0 0 0 0 "Standard tuning"` |
| `real_guitar_22_tuning`      | Specifies a tuning for 22-fret Pro Guitar.                                           | number[4, 5, or 6], string (optional) | `0 2 5 7 10 10` |
| `real_bass_tuning`           | Specifies a tuning for 17-fret Pro Bass.                                             | number[4, 5, or 6], string (optional) | `-2 0 0 0 "Drop D tuning"` |
| `real_bass_22_tuning`        | Specifies a tuning for 22-fret Pro Bass.                                             | number[4, 5, or 6], string (optional) | `0 0 0 0 0` |
| `real_keys_lane_count_right` | Specifies the number of lanes for the right hand in Real Keys.                       | number           | |
| `real_keys_lane_count_left`  | Specifies the number of lanes for the left hand in Real Keys.                        | number           | |
| `eof_midi_import_drum_accent_velocity` | Tells Editor on Fire that drum notes at this velocity should be imported as accents. | number | |
| `eof_midi_import_drum_ghost_velocity`  | Tells Editor on Fire that drum notes at this velocity should be imported as ghosts.  | number | |

## Documentation Notes

All of these tags are referenced from either [grishhung's song.ini guide](https://docs.google.com/document/d/1ped13di4LqDqhaxbCgZEMUoqnyc3gOy3Bw1FCg58FPI/edit#) or existing song.ini files.
