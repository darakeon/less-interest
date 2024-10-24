from math import ceil
from json import loads

def main(config, print_simulation):
    total_interest = 0
    reinstallments = []

    simulation = []

    currency = config['currency']
    balances_pt = config['balances_pt']
    months = config['months']
    salary = config['salary']
    spent_pt = config['spent_pt']
    spent_br = config['spent_br']
    nubank_limit = config['nubank_limit']
    nubank_installments = config['nubank_installments']
    c6_limit = config['c6_limit']
    c6_installments = config['c6_installments']
    interests = config['interests']
    initial_installments_counts = config['initial_installments_counts']
    initial_installments_delays = config['initial_installments_delays']

    for m, month in enumerate(months):
        simulation.append([])

        if len(c6_installments) <= m:
            c6_installments.append(0)

        if len(reinstallments) <= m:
            reinstallments.append(0)

        simulation[m].append(f"month: {month}")

        simulation[m].append(f"nubank_installments: {nubank_installments[m]}")
        nubank_limit = nubank_limit + nubank_installments[m]
        simulation[m].append(f"nubank_limit: {nubank_limit:.2f}")

        simulation[m].append(f"c6_installments: {c6_installments[m]}")
        c6_limit = c6_limit + c6_installments[m]
        simulation[m].append(f"c6_limit: {c6_limit}")

        limit = nubank_limit + c6_limit
        simulation[m].append(f"limit: {limit:.2f}")

        simulation[m].append(f"salary: {salary[m]}")
        simulation[m].append(f"spent_pt: {spent_pt[m]}")

        balance_pt = balances_pt[m]

        simulation[m].append(f"balance_pt: {balance_pt}")

        balance_pt = balance_pt + salary[m] - spent_pt[m]
        simulation[m].append(f"balance_pt_left: {balance_pt:.2f}")

        balance_pt_br = balance_pt * currency
        simulation[m].append(f"balance_pt_br: {balance_pt_br:.2f}")

        simulation[m].append(f"salary_br: {0}")
        simulation[m].append(f"spent_br: {spent_br[m]}")
        simulation[m].append(f"nubank_installments: {nubank_installments[m]}")
        simulation[m].append(f"c6_installments: {c6_installments[m]}")
        simulation[m].append(f"reinstallments: {reinstallments[m]:.2f}")

        balance_br = spent_br[m] + nubank_installments[m] + c6_installments[m] + reinstallments[m]
        simulation[m].append(f"balance_br: {balance_br:.2f}")

        if balance_br > balance_pt_br:
            reinstallment = balance_br - balance_pt_br

            installment_count = initial_installments_counts[m]
            installment_delay = initial_installments_delays[m]
            interest = interests[installment_count-1][installment_delay]

        else:
            reinstallment = 0
            installment_count = 1
            installment_delay = 0
            interest = 0

        simulation[m].append(f"reinstallent: {reinstallment:.2f}")

        reinstallent_total = ceil((reinstallment * interest) / installment_count * 100) * installment_count / 100

        if reinstallent_total > limit:
            reinstallent_total = limit
            reinstallment = reinstallent_total / interest

        total_interest += (reinstallent_total - reinstallment)

        reinstallent_part = reinstallent_total / installment_count

        simulation[m].append(f"reinstallent_total: {reinstallent_total:.2f}")
        simulation[m].append(f"reinstallent_part: {reinstallent_part:.2f}")

        balance_pt = round((balance_pt_br - balance_br + reinstallment) / currency, 2)
        balances_pt.append(balance_pt)

        nubank_limit -= reinstallent_total

        next_reinstallment = m + 1 + installment_delay
        while(len(reinstallments) <= next_reinstallment + installment_count):
            reinstallments.append(0)

        for i in range(installment_count):
            reinstallments[i + next_reinstallment] += reinstallent_part

    simulation.append(f"total_interest: {total_interest:.2f}")

    if print_simulation:
        for month in simulation[:-1]:
            for data in month:
                print(data)
            print()
        print(simulation[-1])

    return simulation

    #for row in table:
    #    for cel in row:
    #        if isinstance(cel, float):
    #            simulation[m].append(f"{cel:.2f}", end=" ")
    #        else:
    #            simulation[m].append(cel, end=" ")
    #    simulation[m].append()

with open('config.json') as file:
    config = loads(file.read())

main(config, False)
