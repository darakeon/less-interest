use std::fs;


pub struct Config {
    currency: f32,

	balance_pt: f32,
	balance_br: f32,
	tolerance: f32,

	months: Vec<String>,

	salary: Vec<f32>,

	spent_pt: Vec<f32>,
	spent_br: Vec<f32>,

	nubank_limit: f32,
	nubank_installments: Vec<f32>,

	c6_limit: f32,
	c6_installments: Vec<f32>,

	interests: Vec<Vec<f32>>,

	initial_installments_counts: Vec<i16>,
	initial_installments_delays: Vec<i16>,
}

impl Config {
	fn init() -> Config {
		let path_borrowed = "config.json";
		let content = fs::read_to_string(path_borrowed)
			.expect(path_borrowed);
		let config = json::parse(&content).unwrap();

		Config {
			currency: config["currency"].as_f32().unwrap(),

			balance_pt: config["balance_pt"].as_f32().unwrap(),
			balance_br: config["balance_br"].as_f32().unwrap(),
			tolerance: config["tolerance"].as_f32().unwrap(),
		
			months: config["months"].as_array(),
		
			/*
			salary: config["salary"].as_Vec<f32>().unwrap(),
		
			spent_pt: config["spent_pt"].as_Vec<f32>().unwrap(),
			spent_br: config["spent_br"].as_Vec<f32>().unwrap(),
		
			nubank_limit: config["nubank_limit"].as_f32().unwrap(),
			nubank_installments: config["nubank_installments"].as_Vec<f32>().unwrap(),
		
			c6_limit: config["c6_limit"].as_f32().unwrap(),
			c6_installments: config["c6_installments"].as_Vec<f32>().unwrap(),
		
			interests: config["interests"].as_Vec<Vec<f32>>().unwrap(),
		
			initial_installments_counts: config["initial_installments_counts"].as_Vec<i16>().unwrap(),
			initial_installments_delays: config["initial_installments_delays"].as_Vec<i16>().unwrap(),
			*/
		}
	}

	pub fn generate_balances_pt() {

	}
}
