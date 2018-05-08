export class GeoMath {
    static deg2Rad(deg) {
        return deg * Math.PI / 180.0;
    }
    static rad2Deg(rad) {
        return rad / Math.PI * 180.0;
    }
    static WGS84EarthRadius(lat) {
        // http://en.wikipedia.org/wiki/Earth_radius
        const WGS84_a = 6378137.0; // Major semiaxis [m]
        const WGS84_b = 6356752.3; // Minor semiaxis [m]+
        const An = WGS84_a * WGS84_a * Math.cos(lat);
        const Bn = WGS84_b * WGS84_b * Math.sin(lat);
        const Ad = WGS84_a * Math.cos(lat);
        const Bd = WGS84_b * Math.sin(lat);
        return Math.Sqrt((An * An + Bn * Bn) / (Ad * Ad + Bd * Bd));
    }
}

export class GeoBounds {
    constructor(min = null, max = null, center = null) {
        this.min = min || new GeoCoordinate();
        this.max = max || new GeoCoordinate();
        this.center = center || new GeoCoordinate();
    }
}

export class GeoCoordinate {
    constructor(lat = null, long = null, alt = null) {
        this.latitude = lat || 0.0;
        this.longitude = long || 0.0;
        this.altitude = alt || 0.0;
    }

    getBoundingBox(halfSideInKm) {
        let lat = GeoMath.deg2Rad(this.latitude);
        let lon = GeoMath.deg2Rad(this.longitude);
        let halfSide = 1000 * halfSideInKm; // to meters


        // Radius of Earth at given latitude
        let radius = GeoMath.WGS84EarthRadius(lat);

        // Radius of the parallel at given latitude
        let pradius = radius * Math.cos(lat);
        let latMin = lat - halfSide / radius;
        let latMax = lat + halfSide / radius;
        let lonMin = lon - halfSide / pradius;
        let lonMax = lon + halfSide / pradius;
        return new GeoBounds(
            new GeoCoordinate(GeoMath.rad2Deg(latMin), GeoMath.rad2Deg(lonMin)),
            new GeoCoordinate(GeoMath.rad2Deg(latMax), GeoMath.rad2Deg(lonMax)),
            this
        );
    }
}

export class Geo {
    static toScreenCoordinates(bounds, width, height, point) {
        const posX = ((point.latitude - bounds.min.longitude) / (bounds.max.longitude - bounds.min.longitude)) * width;
        const posY = ((point.longitude - bounds.min.latitude) / (bounds.max.latitude - bounds.min.latitude)) * height;
        return { x: posX, y: posY};
    }
}


// usage:
//   toScreenCoordinates(
//                       bounds: GeoBounds - calculated using origin point/our current position and call getBoundingBox(1.0)
//                       width : our canvas width, where the target point relates to.
//                       height: our canvas height
//                       point : target geocoordinate, its the node position
// Example code: (NOT TESTED!!!)

const canvas = document.querySelector("canvas");
const ctx    = canvas.getContext("2d");
ctx.width  = 500;
ctx.height = 150;

// list of nodes received from server:
const ourPlayerPosition = new GeoCoordinate(-81.26171875, 28.977834701538);
const nodes = [
    {
        position: new GeoCoordinate(-81.26171875, 28.977834701538)
    }
];

const bounds = ourPlayerPosition.getBoundingBox(1.0); // 2km radius
for(const node of nodes) {
    const screenPos = Geo.toScreenCoordinates(bounds, ctx.width, ctx.height, node.position);
    ctx.ellipse(screenPos.x, screenPos.y, 5, 5, 0, 0, Math.PI * 180);
}

ctx.fillStyle = "red";
ctx.fill();

