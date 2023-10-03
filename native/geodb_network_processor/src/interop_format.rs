#[repr(C)]
pub struct InteropNetworkEntry  {
    pub country_code: [u8; 2],
    pub network: u128,
    pub cidr: u8
}

#[repr(C)]
pub struct InteropNetworkRange {
    pub start: u128,
    pub end: u128,

    pub cc: [u8; 2]
}

#[repr(C)]
pub struct InteropSortResult {
    pub networks: *const InteropNetworkRange,
    pub count: usize
}