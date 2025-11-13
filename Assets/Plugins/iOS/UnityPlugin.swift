import Foundation
import CoreLocation
import UIKit

@MainActor
@_cdecl("startBeaconStream")
public func startBeaconStream() {
    let stream = BeaconStream(uuids: [
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF1")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF2")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF3")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF4")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF5")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF6")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF7")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF8")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDF9")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDFA")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDFB")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDFC")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDFD")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDFE")!,
        UUID(uuidString: "1B6295D5-4F74-4C58-A2D8-CD83CA26BDFF")!
    ])
    stream.start()
}

