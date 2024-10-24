from math import ceil
from json import loads

#table = []

def main():
    with open('config.json') as file:
        config = loads(file.read())
        print(config)

    total_interest = 0
    reinstallments = []

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
        #table.append([])

        if len(c6_installments) <= m:
            c6_installments.append(0)

        if len(reinstallments) <= m:
            reinstallments.append(0)

        print(f"month: {month}")
        #table[m].append(month)

        print(f"nubank_installments: {nubank_installments[m]}")
        #table[m].append(nubank_installments[m])
        nubank_limit = nubank_limit + nubank_installments[m]
        print(f"nubank_limit: {nubank_limit:.2f}")
        #table[m].append(nubank_limit)

        print(f"c6_installments: {c6_installments[m]}")
        #table[m].append(c6_installments[m])
        c6_limit = c6_limit + c6_installments[m]
        print(f"c6_limit: {c6_limit}")
        #table[m].append(c6_limit)

        limit = nubank_limit + c6_limit
        print(f"limit: {limit:.2f}")
        #table[m].append(limit)

        print(f"salary: {salary[m]}")
        #table[m].append(salary[m])
        print(f"spent_pt: {spent_pt[m]}")
        #table[m].append(spent_pt[m])

        balance_pt = balances_pt[m]

        print(f"balance_pt: {balance_pt}")
        #table[m].append(balance_pt)

        balance_pt = balance_pt + salary[m] - spent_pt[m]
        print(f"balance_pt_left: {balance_pt:.2f}")
        #table[m].append(balance_pt)

        balance_pt_br = balance_pt * currency
        print(f"balance_pt_br: {balance_pt_br:.2f}")
        #table[m].append(balance_pt_br)

        print(f"salary_br: {0}")
        #table[m].append(0)
        print(f"spent_br: {spent_br[m]}")
        #table[m].append(spent_br[m])
        print(f"nubank_installments: {nubank_installments[m]}")
        #table[m].append(nubank_installments[m])
        print(f"c6_installments: {c6_installments[m]}")
        #table[m].append(c6_installments[m])
        print(f"reinstallments: {reinstallments[m]:.2f}")
        #table[m].append(reinstallments[m])

        balance_br = spent_br[m] + nubank_installments[m] + c6_installments[m] + reinstallments[m]
        print(f"balance_br: {balance_br:.2f}")
        #table[m].append(balance_br)

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

        print(f"reinstallent: {reinstallment:.2f}")
        #table[m].append(reinstallment)

        reinstallent_total = ceil((reinstallment * interest) / installment_count * 100) * installment_count / 100

        if reinstallent_total > limit:
            reinstallent_total = limit
            reinstallment = reinstallent_total / interest

        total_interest += (reinstallent_total - reinstallment)

        reinstallent_part = reinstallent_total / installment_count

        print(f"reinstallent_total: {reinstallent_total:.2f}")
        #table[m].append(reinstallent_total)
        print(f"reinstallent_part: {reinstallent_part:.2f}")
        #table[m].append(reinstallent_part)

        balance_pt = round((balance_pt_br - balance_br + reinstallment) / currency, 2)
        balances_pt.append(balance_pt)

        #reinstallments[m+1] += reinstallment
        nubank_limit -= reinstallent_total

        next_reinstallment = m + 1 + installment_delay
        while(len(reinstallments) <= next_reinstallment + installment_count):
            reinstallments.append(0)

        for i in range(installment_count):
            reinstallments[i + next_reinstallment] += reinstallent_part

        print()

    print(f"total_interest: {total_interest:.2f}")

    #for row in table:
    #    for cel in row:
    #        if isinstance(cel, float):
    #            print(f"{cel:.2f}", end=" ")
    #        else:
    #            print(cel, end=" ")
    #    print()

main()
