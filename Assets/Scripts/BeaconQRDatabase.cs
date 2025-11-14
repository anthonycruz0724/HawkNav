using System;
using System.Collections.Generic;

[Serializable]
public class BeaconQrEntry
{
    public string shortname;
    public int minor;
}

[Serializable]
public class BeaconQrDatabase
{
    public List<BeaconQrEntry> beacons = new List<BeaconQrEntry>();
}
