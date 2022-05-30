import React, { useState, useEffect } from 'react';
import classes from './Booking.module.css';

import { useParams } from 'react-router-dom';

import axios from 'axios';

import { format, isEqual } from 'date-fns';
import { uk } from 'date-fns/locale';

import { capitalize } from 'lodash';
import classNames from 'classnames';

import GeneratedTicketDetail from './GeneratedTicketDetail';
import Button from '../../components/Button/Button';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';

interface HallLayout {
    id: number;
    title: string;
    layoutRows: HallLayoutRow[];
}

interface HallLayoutRow {
    id: number;
    layoutItems: HallLayoutItem[];
}

interface HallLayoutItem {
    rowID: number;
    colID: number;
    type: HallLayoutItemType;
}

enum HallLayoutItemType {
    EMPTY = 0,
    COMMON = 1,
    VIP = 2,
    COUCH = 3,
}

const widthRegistry = {
    [HallLayoutItemType.EMPTY]: 1,
    [HallLayoutItemType.COMMON]: 1,
    [HallLayoutItemType.VIP]: 1,
    [HallLayoutItemType.COUCH]: 2,
};

function getHallLayout() {
    const layout: HallLayout = {
        id: 1,
        title: 'A',
        layoutRows: generateLayoutRows(),
    };

    return layout;
}

function generateLayoutRows(): HallLayoutRow[] {
    const empty = HallLayoutItemType.EMPTY;
    const common = HallLayoutItemType.COMMON;
    const vip = HallLayoutItemType.VIP;
    const couch = HallLayoutItemType.COUCH;

    // prettier-ignore
    const simplifiedRows = [
        [empty, common, common, common, common, empty],
        [empty, empty , vip   , vip   , empty , empty],
        [empty, empty , couch         , empty , empty]
    ]

    const rows: HallLayoutRow[] = simplifiedRows.map((row, rowID) => {
        return {
            id: rowID,
            layoutItems: row.map((item, colID) => {
                return {
                    rowID: rowID,
                    colID: colID,
                    type: item,
                };
            }),
        };
    });

    return rows;
}

async function getAvailableForBookingSessions(filmID) {
    const response = await axios.get(
        `${window.apiBaseUrl}/sessions/available-for-booking/${filmID}`
    );

    return response.data.sessions.map((session) => {
        return {
            ...session,
            startAt: new Date(session.startAt),
        };
    });
}

function computeSessionTimeHierarchy(sessions) {
    return sessions.reduce((accYears, session) => {
        const year = session.startAt.getUTCFullYear();

        if (!accYears[year]) {
            accYears[year] = {};
        }
        const correspondingYear = accYears[year];

        const month = session.startAt.getUTCMonth();
        if (!correspondingYear[month]) {
            correspondingYear[month] = {};
        }
        const correpondingMonth = correspondingYear[month];

        const date = session.startAt.getUTCDate();
        if (!correpondingMonth[date]) {
            correpondingMonth[date] = [];
        }
        const correspondingDate = correpondingMonth[date];

        correspondingDate.push({
            startAt: session.startAt,
            hallID: session.hallID,
            hallTitle: session.hallTitle,
        });

        return accYears;
    }, {});
}

function computeAvailableDates(sessionTimeHierarchy) {
    return Object.keys(sessionTimeHierarchy).flatMap((year) => {
        return Object.keys(sessionTimeHierarchy[year]).flatMap((month) => {
            return Object.keys(sessionTimeHierarchy[year][month]).flatMap(
                (date) => {
                    return {
                        date: new Date(year, month, date),
                        availableTimes: sessionTimeHierarchy[year][month][date],
                    };
                }
            );
        });
    });
}

function getHallItemTypeClass(itemType: HallLayoutItemType) {
    switch (itemType) {
        case HallLayoutItemType.COMMON:
            return classes['hall-item-common'];
        case HallLayoutItemType.VIP:
            return classes['hall-item-vip'];
        case HallLayoutItemType.COUCH:
            return classes['hall-item-couch'];
        case HallLayoutItemType.EMPTY:
        default:
            return classes['hall-item-empty'];
    }
}

const Booking = (props) => {
    const params = useParams();

    const title = 'Spider-Man: No Way Home';
    const [availableDates, setAvailableDates] = useState([]);
    const [selectedDate, setSelectedDate] = useState(null);
    const [selectedTime, setSelectedTime] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    const layout = getHallLayout();

    useEffect(async () => {
        const fetchedAvailableSessions = await getAvailableForBookingSessions(
            params.filmId
        );

        const sessionTimeHierarchy = computeSessionTimeHierarchy(
            fetchedAvailableSessions
        );
        let computedAvailableDates =
            computeAvailableDates(sessionTimeHierarchy);
        setAvailableDates(computedAvailableDates);

        if (computedAvailableDates && computedAvailableDates.length > 0) {
            const newSelectedDate = computedAvailableDates[0];
            setSelectedDate(newSelectedDate);

            if (newSelectedDate && newSelectedDate.availableTimes.length > 0) {
                setSelectedTime(newSelectedDate.availableTimes[0]);
            } else {
                setSelectedTime(null);
            }
        } else {
            setSelectedDate(null);
        }

        setIsLoading(false);
    }, []);

    if (isLoading) {
        return <div>Завантаження...</div>;
    }

    return (
        <div className={classes['booking-layout']}>
            <div className={classes['choosing-area']}>
                <div className={classes['top-row']}>
                    <button className={classes['back-button']}>
                        <FontAwesomeIcon icon={faChevronLeft} />
                    </button>
                    <h1 className={classes['film-title']}>{title}</h1>
                </div>

                <div className={classes['date-choosing-area']}>
                    {availableDates.map((availableDate) => (
                        <div
                            className={classNames(
                                classes['available-date'],
                                availableDate === selectedDate
                                    ? classes['active']
                                    : null
                            )}
                            onClick={() => {
                                setSelectedDate(availableDate);
                            }}
                            key={availableDate.date.getTime()}
                        >
                            {capitalize(
                                format(availableDate.date, 'cccccc, d MMMM', {
                                    locale: uk,
                                })
                            )}
                        </div>
                    ))}
                </div>

                <div className={classes['time-choosing-area']}>
                    {selectedDate.availableTimes.map((availableTime) => (
                        <div
                            className={classNames(
                                classes['available-time'],
                                availableTime === selectedTime
                                    ? classes['active']
                                    : null
                            )}
                            onClick={() => {
                                setSelectedTime(selectedTime);
                            }}
                            key={availableTime.startAt.getTime()}
                        >
                            {capitalize(format(availableTime.startAt, 'HH:mm'))}
                        </div>
                    ))}
                </div>

                <div className={classes['seat-choosing-area']}>
                    <div className={classes['hall-layout']}>
                        {layout.layoutRows.map((layoutRow) => (
                            <div
                                className={classes['hall-layout-row']}
                                key={layoutRow.id}
                            >
                                {layoutRow.layoutItems.map((layoutItem) => (
                                    <div
                                        className={classNames(
                                            classes['hall-layout-item'],
                                            getHallItemTypeClass(
                                                layoutItem.type
                                            )
                                        )}
                                        style={
                                            {
                                                '--hall-item-relative-width':
                                                    widthRegistry[
                                                        layoutItem.type
                                                    ],
                                            } as React.CSSProperties
                                        }
                                    ></div>
                                ))}
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            <div className={classes['generated-ticket']}>
                <div
                    className={classNames(
                        classes['generated-ticket-card'],
                        classes['sidebar-card']
                    )}
                >
                    <h2 className={classes['sidebar-card-title']}>Квиток</h2>
                    <GeneratedTicketDetail title="Фільм" description={title} />
                    <GeneratedTicketDetail
                        title="Час"
                        description={
                            capitalize(
                                format(
                                    selectedTime.startAt,
                                    'cccccc, d MMMM HH:mm',
                                    {
                                        locale: uk,
                                    }
                                )
                            ) ?? ''
                        }
                    />
                    <GeneratedTicketDetail
                        title="Зала"
                        description={selectedTime.hallTitle}
                    />

                    <GeneratedTicketDetail
                        title="Місця"
                        // TODO Make dynamic
                        description="Б8, Б9, Б10"
                    />
                </div>
            </div>

            <div className={classes['checkout']}>
                <div
                    className={classNames(
                        classes['checkout-card'],
                        classes['sidebar-card']
                    )}
                >
                    <h2 className={classes['sidebar-card-title']}>До сплати</h2>
                    Всього: 150 грн
                    <div className={classes['checkout-buttons-container']}>
                        <Button>Купувати квиток</Button>
                        <Button variant="alternative">Бронювати</Button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Booking;
