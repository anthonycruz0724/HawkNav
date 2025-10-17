import Foundation
import CoreLocation
import UIKit

@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>, _ method: UnsafePointer<CChar>, _ msg: UnsafePointer<CChar>)

@MainActor
class BeaconStream: NSObject, CLLocationManagerDelegate {
    private let locationManager = CLLocationManager()
    private let beaconsToTrack: [UUID]
    private var timer: Timer?
    private var lastBeacons: [CLBeacon] = []
    
    init(uuids: [UUID]) {
        self.beaconsToTrack = uuids
        super.init()
        locationManager.delegate = self
        locationManager.requestWhenInUseAuthorization()
        print("Aurthorization status: \(CLLocationManager.authorizationStatus())")
    }

    func start() {
        for uuid in beaconsToTrack {
            let region = CLBeaconRegion(uuid: uuid, identifier: uuid.uuidString)
            locationManager.startRangingBeacons(satisfying: region.beaconIdentityConstraint)
        }
        
        timer = Timer.scheduledTimer(withTimeInterval: 2.0, repeats: true) { _ in
            self.report()
        }
    }
    
    func stop() {
        timer?.invalidate()
        for uuid in beaconsToTrack {
            let region = CLBeaconRegion(uuid: uuid, identifier: uuid.uuidString)
            locationManager.stopRangingBeacons(satisfying: region.beaconIdentityConstraint)
        }
    }
    
    func locationManager(_ manager: CLLocationManager,
                         didRange beacons: [CLBeacon],
                         satisfying constraint: CLBeaconIdentityConstraint) {
        lastBeacons = beacons
    }
    
    private func report() {
        let data = lastBeacons.map {
            [
                "uuid": $0.uuid.uuidString,
                "major": $0.major.intValue,
                "minor": $0.minor.intValue,
                "rssi": $0.rssi,
                "proximity": proximityString($0.proximity)
            ]
        }
        let jsonData = try? JSONSerialization.data(withJSONObject: data, options: .prettyPrinted)
        let jsonString = String(data: jsonData ?? Data(), encoding: .utf8) ?? "[]"
        UnitySendMessage("BeaconManager", "OnBeaconUpdate", jsonString)
    }
    
    private func proximityString(_ proximity: CLProximity) -> String {
        switch proximity {
        case .immediate: return "immediate"
        case .near: return "near"
        case .far: return "far"
        default: return "unknown"
        }
    }
}



