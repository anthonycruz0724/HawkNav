import Foundation
import CoreLocation
import UIKit

@MainActor
@_cdecl("startBeaconStream")
public func startBeaconStream() {
    let stream = BeaconStream(uuids: [
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF3")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF4")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF5")!
    ])
    stream.start()
}

