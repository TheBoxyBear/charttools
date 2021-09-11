# Initialization

Initialization files are commonly-used files for storing things such as settings/configurations. They are text-based and use the .ini format.

Song metadata for .mid files are stored in a song.ini file. Clone Hero also extends this file to .chart files.

Table of contents:

- [Initialization](#initialization)
  - [Basic Structure](#basic-structure)
    - [Sections](#sections)
    - [Tags](#tags)
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

`Key` is a string, and `Value` is either a string, number, or a boolean, depending on the key. No specific number types to my knowledge, so it'd be safest to use at least a 32-bit signed integer for each.

The equals sign may, but is not required to be, padded by spaces.

## Available Tags

There are *many* tags in existance, so listing them all in one place is rather difficult. Still, this should be a pretty good amount of them. Additionally, tags can be in any order in the song.ini, so you should *not* rely on the order of tags to determine what is what.

### Common Tags

| `Key`                      | Description                                                                                             | Data type |
| :------------------------: | :------------------------------------------------------------------------------------------------------ | :-------: |
| `name`                     | Title of the song.                                                                                      |  string   |
| `artist`                   | Artist or band behind the song.                                                                         |  string   |
| `album`                    | Title of the album the song is featured in.                                                             |  string   |
| `genre`                    | Genre of the song.                                                                                      |  string   |
| `year`                     | Year of the song’s release.                                                                             |  string   |
| `album_track`              | Track number of the song within the album it's from.                                                    |  number   |
| `track`                    | Same as `album_track`. (Legacy)                                                                         |  number   |
| `playlist_track`           | Track number of the song within the playlist/setlist it's from.                                         |  number   |
| `charter`                  | Community member responsible for charting the song.                                                     |  string   |
| `frets`                    | Same as `charter`. (Legacy)                                                                             |  string   |
| `preview_start_time`       | Time of the song, in milliseconds, where the song preview starts.                                       |  number   |
| `video_start_time`         | Time of the video, in milliseconds, where playback of an included video background will start.          |  number   |
| `diff_guitar`              | Estimated difficulty of the Lead Guitar track.                                                          |  number   |
| `diff_guitarghl`           | Estimated difficulty of the 6 Fret Lead track.                                                          |  number   |
| `diff_bass`                | Estimated difficulty of the Bass Guitar track.                                                          |  number   |
| `diff_bassghl`             | Estimated difficulty of the 6 Fret Bass track.                                                          |  number   |
| `diff_rhythm`              | Estimated difficulty of the Rhythm Guitar track.                                                        |  number   |
| `diff_keys`                | Estimated difficulty of the Keys track.                                                                 |  number   |
| `diff_drums`               | Estimated difficulty of the Drums track.                                                                |  number   |
| `pro_drums`                | Forces the Drums track to be read as Pro Drums charted regardless of tom/cymbal markers being detected. |  boolean  |
| `five_lane_drums`          | Forces the Drums track to be read as 5-lane regardless of if 5-lane Green is detected.                  |  boolean  |
| `icon`                     | Name of the charter icon to display for this song on the main menu.                                     |  string   |
| `song_length`              | Length of the song's audio in milliseconds.                                                             |  number   |
| `delay`                    | Delay time of the chart in milliseconds. A higher value makes the chart start later. This is not recommended to be used by today’s charting standards, and is maintained for backwards compatibility. |  number   |
| `modchart`                 | Indicates if this song is a modchart.                                                                   |  boolean  |
| `loading_phrase`           | Text that will be shown during instrument/difficulty/modifier selection.                                |  string   |
| `sustain_cutoff_threshold` | Overrides the default threshold under which a sustain will get removed.                                 |  number   |
| `hopo_frequency`           | Overrides the natural HOPO threshold.                                                                   |  number   |
| `end_events`               | Overrides whether or not end events in the chart will be respected.                                     |  boolean  |

### Legacy/Unused Tags

(Some of these descriptions may be wrong.)

| `Key`                 | Description                                                                     | Data type |
| :-------------------: | :------------------------------------------------------------------------------ | :-------: |
| `diff_band`           | Estimated average difficulty of all tracks in the song.                         |  number   |
| `diff_guitar_coop`    | Estimated difficulty of the Guitar Co-op track.                                 |  number   |
| `diff_guitar_real`    | Estimated difficulty of the Pro Guitar track.                                   |  number   |
| `diff_guitar_real_22` | Estimated difficulty of the Pro Guitar 22-fret track.                           |  number   |
| `diff_bass_real`      | Estimated difficulty of the Pro Bass track.                                     |  number   |
| `diff_bass_real_22`   | Estimated difficulty of the Pro Bass 22-fret track.                             |  number   |
| `diff_drums_real`     | Estimated difficulty of the Pro Drums track.                                    |  number   |
| `diff_drums_real_ps`  | Estimated difficulty of the Drums Real track.                                   |  number   |
| `diff_keys_real`      | Estimated difficulty of the Pro Keys track.                                     |  number   |
| `diff_keys_real_ps`   | Estimated difficulty of the Keys Real track.                                    |  number   |
| `diff_vocals`         | Estimated difficulty of the Vocals track.                                       |  number   |
| `diff_vocals_harm`    | Estimated difficulty of the Harmonies tracks.                                   |  number   |
| `diff_dance`          | Estimated difficulty of the Dance track.                                        |  number   |
| `sysex_slider`        | Indicates that this chart has SysEx events for sliders/tap notes.               |  boolean  |
| `sysex_high_hat_ctrl` | Indicates that this chart has SysEx events for Drums Real hi-hat pedal control. |  boolean  |
| `sysex_rimshot`       | Indicates that this chart has SysEx events for Drums Real rimshot hits.         |  boolean  |
| `sysex_open_bass`     | Indicates that this chart has SysEx events for open notes.                      |  boolean  |
| `banner_link_a`       | Link A for an in-game banner?                                                   |  string   |
| `link_name_a`         | Name for Link A?                                                                |  string   |
| `banner_link_b`       | Link B for an in-game banner?                                                   |  string   |
| `link_name_b`         | Name for Link B?                                                                |  string   |
| `video`               | Name/path for a video file?                                                     |  string   |

## Documentation Notes

A large number of these tags are referenced from [grishhung's song.ini guide](https://docs.google.com/document/d/1ped13di4LqDqhaxbCgZEMUoqnyc3gOy3Bw1FCg58FPI/edit#).
